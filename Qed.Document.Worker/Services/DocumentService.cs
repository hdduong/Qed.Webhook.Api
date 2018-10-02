using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using Qed.Document.Worker.Interfaces;
using Qed.Webhook.Api.Repository.Entities;
using Qed.Webhook.Api.Repository.Interfaces;
using Qed.Webhook.Api.Shared.Bus.Document.Events;
using Qed.Webhook.Api.Shared.Cache;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Api.Shared.Enums;
using Qed.Webhook.Api.Shared.Helpers;
using Qed.Webhook.Api.Shared.Models.Document;
using Qed.Webhook.Service.Interfaces;
using Qed.Webhook.Service.Models;
using Qed.Webhook.Service.Models.Requests.AccessToken;
using Qed.Webhook.Service.Models.Requests.Document;
using Qed.Webhook.Service.Models.Responses;
using Qed.Webhook.Service.Models.Responses.AccessToken;
using Qed.Webhook.Service.Models.Responses.Document;
using IDocumentWorkerConfiguration = Qed.Document.Worker.Interfaces.IDocumentWorkerConfiguration;

namespace Qed.Document.Worker.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ILocalCache _cache;
        private readonly IEncompassClient _encompassClient;
        private readonly IRedisCacheApiClient _redisCacheClient;
        private readonly IEncompassDocumentRepository _documentRepository;
        private readonly IDocumentWorkerConfiguration _documentWorkerConfiguration;

        public DocumentService(IEncompassClient encompassClient, IRedisCacheApiClient redisCacheClient, ILocalCache cache, 
            IEncompassDocumentRepository documentRepository, IDocumentWorkerConfiguration documentWorkerConfiguration)
        {
            _encompassClient = encompassClient;
            _redisCacheClient = redisCacheClient;
            _cache = cache;
            _documentRepository = documentRepository;
            _documentWorkerConfiguration = documentWorkerConfiguration;
        }

        public async Task<ProcessDocumentResponse> ProcessDocumentAsync(ProcessDocumentRequest request)
        {
            var response = new ProcessDocumentResponse();

            try
            {
                var sdkConfig = _cache.Get<EncompassSdkConfig>(ConstantString.EncompassSdkCacheKey);
                var tokenResponse = await RequestAccessToken(sdkConfig).ConfigureAwait(false);
                
                // get access token
                if (!tokenResponse.StatusCode.IsSuccessStatusCode())
                {
                    response.SuccessBit = false;
                    response.EventStatus = EventStatusEnum.Error;
                    response.ErrorMsgTxt = JsonConvert.SerializeObject(new
                    {
                        errorCode = tokenResponse.StatusCode,
                        errorMsgTxt = JsonConvert.SerializeObject(tokenResponse.Body.FailureResponse)
                    });
                }
                var accessToken = tokenResponse.Body.SuccessResponse.AccessToken;

                // get list of latest documents via api
                var getAttachmentApis = await _encompassClient.QueryDocumentAsync(new GetAttachmentRequest
                {
                    AccessToken = accessToken,
                    LoanGuid = request.LoanGuid
                }).ConfigureAwait(false);

                if (getAttachmentApis.Body?.FailureResponse != null)
                {
                    response.SuccessBit = false;
                    response.EventStatus = EventStatusEnum.Error;
                    response.ErrorMsgTxt = JsonConvert.SerializeObject(new
                    {
                        errorCode = getAttachmentApis.StatusCode,
                        errorMsgTxt = string.Format(ConstantString.ErrorWhileGettingAttachments,
                            JsonConvert.SerializeObject(getAttachmentApis.Body?.FailureResponse))
                    });
                }

                var cachedDocuments = await GetDocumentCacheAsync(request.LoanGuid).ConfigureAwait(false);
                var apiDocuments = ParseDoucmentApiResponse(getAttachmentApis.Body);
                var apiDocumentIds = apiDocuments.Select(x => x.Key).ToHashSet();
                var apiNotCacheDocumentIds = GetDocumentIdToDownload(cachedDocuments, apiDocumentIds);

                // latest documents from api the same as cache
                if (apiNotCacheDocumentIds.Count == 0)
                {
                    response.SuccessBit = true;
                    response.EventStatus = EventStatusEnum.Done;
                    response.EventReason = string.Format(ConstantString.AllDocumentDowloaded);
                    return response;
                }

                // here means there are mismatch between documents in cache and in api
                var latestPersistedDocuments = await GetPersistedDocumentAsync(request.LoanGuid).ConfigureAwait(false);
                var apiNotDbDocumentIds = GetDocumentIdToDownload(latestPersistedDocuments, apiNotCacheDocumentIds);

                // there is no new documents
                if (apiNotDbDocumentIds.Count == 0)
                {
                    response = await TryUpdateDocumentCache(request.LoanGuid, latestPersistedDocuments, response).ConfigureAwait(false);
                    return response;
                }

                // in db, we do not even have documents so we need to perform download.
                var addingDbResults = await AddNewDocument(apiNotDbDocumentIds, apiDocuments, request.LoanGuid).ConfigureAwait(false);
                var successAddingDbResults = ExtractSuccessDbResult(addingDbResults, response.DocErrors, false);

                var downloadDocumentResponses = await PerformDownloadDocument(successAddingDbResults, request.LoanGuid, accessToken).ConfigureAwait(false);
                var downloadSuccessResponses = ExtractSuccessApiResult(downloadDocumentResponses, response.DocErrors, false);

                var updatingDbResults = await UpdateNewDocument(downloadSuccessResponses, request.LoanGuid);
                var updatedSuccessDbResults = ExtractSuccessDbResult(updatingDbResults, response.DocErrors, true);

                var updatededDocumentGuids = GetUpdatedDocumentGuid(updatedSuccessDbResults);

                if (cachedDocuments != null)
                {
                    updatededDocumentGuids.UnionWith(cachedDocuments);
                }
                response = await TryUpdateDocumentCache(request.LoanGuid, updatededDocumentGuids, response).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                response.SuccessBit = false;
                response.EventStatus = EventStatusEnum.Error;
                response.EventReason = ex.Message;
                response.ErrorMsgTxt = JsonConvert.SerializeObject(new { message = JsonConvert.SerializeObject(ex.Message), stackTrace = ex.StackTrace });
            }

            return response;
        }


        private async Task<DbResponse<EncompassDocumentEntity>[]> UpdateNewDocument(ApiResponse<DownloadAttachmentResponse>[] downloadedDocuments, Guid loanGuid)
        {
            var updatingDbDocs = downloadedDocuments.Select(x =>
            {
                var document = x.Body.EncompassDocument;
                return new EncompassDocumentEntity
                {
                    Id = document.Id,
                    LoanGuid = loanGuid,
                    DocumentTitle = document.DocumentTitle,
                    DocumentTypeName = document.DocumentTypeName,
                    DocumentDtTm = document.DocumentDtTm,
                    DocumentGuid = document.DocumentGuid,
                    DocumentId = document.DocumentId,
                    AddedById = document.AddedById,
                    AddedByName = document.AddedByName,
                    FileName = document.FileName,
                    FileSizeBytes = document.FileSizeBytes,
                    DownloadSucceededBit = x.Body.IsSuccessBit,
                    DownloadFileFullPath = x.Body.DownloadFullPath,
                    StartDownloadDtTm = document.StartDownloadDtTm,
                    EndDownloadDtTm = DateTimeOffset.Now,
                    IsActiveBit = document.IsActiveBit,
                    DownloadStatusTypeId = x.Body.IsSuccessBit ? DocumentStatusEnum.Done : DocumentStatusEnum.Error,
                    ErrorMsgTxt = x.Body.IsSuccessBit ? null : x.Body.FailureResponse.ToString()
                };
            });
            var updatingDbTasks = updatingDbDocs.Select(x => _documentRepository.UpdateDocumentAsync(x));
            var updatingDbResults = await Task.WhenAll(updatingDbTasks.ToArray()).ConfigureAwait(false);
            return updatingDbResults;
        }



        private async Task<ApiResponse<DownloadAttachmentResponse>[]> PerformDownloadDocument(DbResponse<EncompassDocumentEntity>[] addedDocuments, Guid loanGuid, string accessToken)
        {
            var urlRequests = addedDocuments.Select(x => new DownloadAttachmentRequest
            {
                LoanId = EncompassHelper.GetLoanId(loanGuid),
                AccessToken = accessToken,
                EncompassDocument = x.Body,
                DownloadFullPath = BuildDownloadFilePath(_documentWorkerConfiguration.GetDownloadPath(),
                    x.Body.LoanGuid, x.Body.DocumentTypeName, x.Body.DocumentId)
            });

            var downloadTasks = urlRequests.Select(x => _encompassClient.DownloadAttachmentAsync(x));
            var downloadResponses = await Task.WhenAll(downloadTasks.ToArray()).ConfigureAwait(false);
            return downloadResponses;
        }

        private async Task<DbResponse<EncompassDocumentEntity>[]> AddNewDocument(HashSet<Guid> addingDocumentGuids, Dictionary<Guid, Attachment> apiDocuments, Guid loanGuid)
        {
            var addingDbDocs = addingDocumentGuids.Select(x =>
            {
                var document = apiDocuments[x];
                return new EncompassDocumentEntity
                {
                    LoanGuid = loanGuid,
                    DocumentTitle = document.Document.EntityName,
                    DocumentTypeName = EncompassHelper.GetAlphaNumeric(document.Document.EntityName),
                    DocumentDtTm = document.DateCreated,
                    DocumentGuid = x,
                    DocumentId = document.AttachmentId,
                    AddedById = document.CreatedBy,
                    AddedByName = document.CreatedByName,
                    FileName = document.Title,
                    FileSizeBytes = document.FileSize,
                    DownloadSucceededBit = false,
                    DownloadFileFullPath = string.Empty,
                    StartDownloadDtTm = DateTimeOffset.Now,
                    EndDownloadDtTm = null,
                    IsActiveBit = document.IsActive,
                    DownloadStatusTypeId = DocumentStatusEnum.Downloading,
                    ErrorMsgTxt = null
                };
            });

            var addingDbTasks = addingDbDocs.Select(x => _documentRepository.AddDocumentAsync(x));
            var addingDbResults = await Task.WhenAll(addingDbTasks.ToArray()).ConfigureAwait(false);
            return addingDbResults;
        }


        private async Task<ApiResponse<GetTokenResponse>> RequestAccessToken(EncompassSdkConfig sdkConfig)
        {
            var tokenResponse = await _encompassClient.GetSecurityTokenAsync(new GetTokenRequest
            {
                ClientId = sdkConfig.ClientId,
                ClientSecret = sdkConfig.ClientSecret,
                InstanceId = sdkConfig.InstanceId,
                SmartClientPassword = sdkConfig.DecryptedPassword,
                SmartClientUserName = sdkConfig.UserId
            }).ConfigureAwait(false);

            return tokenResponse;
        }

        private async Task<ProcessDocumentResponse> TryUpdateDocumentCache(Guid loanGuid, HashSet<Guid> documentGuids, ProcessDocumentResponse response)
        {
            response.SuccessBit = await SetDocumentCacheAsync(loanGuid, documentGuids).ConfigureAwait(false);
            if (response.SuccessBit)
            {
                response.EventStatus = EventStatusEnum.Done;
                response.EventReason = string.Format(ConstantString.AllDocumentDowloaded);
            }
            else
            {
                response.EventStatus = EventStatusEnum.CacheOutOfSync;
                response.EventReason = string.Format(ConstantString.CacheIssueAllDocumentDowloaded);
            }

            return response;
        }

        private HashSet<Guid> GetUpdatedDocumentGuid(DbResponse<EncompassDocumentEntity>[] dbResponses)
        {
           return dbResponses.Select(x => x.Body.DocumentGuid).ToHashSet();
        }

        private ApiResponse<DownloadAttachmentResponse>[] ExtractSuccessApiResult(
            ApiResponse<DownloadAttachmentResponse>[] apiResponses, ConcurrentDictionary<Guid, DocumentErrorDtl> docErrors, bool documentDownloadedBit)
        {
            var responses = new List<ApiResponse<DownloadAttachmentResponse>>();
            foreach (var apiResponse in apiResponses)
            {
                if (apiResponse.Body.IsSuccessBit)
                {
                    responses.Add(apiResponse);
                }
                else
                {
                    var errorDtl = new DocumentErrorDtl
                    {
                        IsDownloadSuccessBit = documentDownloadedBit,
                        ErrorMsgTxt = JsonConvert.SerializeObject(apiResponse.Body.FailureResponse)
                    };
                    docErrors.TryAdd(apiResponse.Body.EncompassDocument.DocumentGuid, errorDtl);
                }
            }

            return responses.ToArray();
        }


        private DbResponse<EncompassDocumentEntity>[] ExtractSuccessDbResult(DbResponse<EncompassDocumentEntity>[] dbResponses, ConcurrentDictionary<Guid, DocumentErrorDtl> docErrors, bool documentDownloadedBit)
        {
            var responses = new List<DbResponse<EncompassDocumentEntity>>();
            foreach (var dbResponse in dbResponses)
            {
                if (dbResponse.IsSuccessBit)
                {
                    responses.Add(dbResponse);
                }
                else
                {
                    var errorDtl = new DocumentErrorDtl
                    {
                        IsDownloadSuccessBit = documentDownloadedBit,
                        ErrorMsgTxt = dbResponse.ErrorMsgTxt
                    };
                    docErrors.TryAdd(dbResponse.Body.DocumentGuid, errorDtl);
                }
            }

            return responses.ToArray();
        }

        private string BuildDownloadFilePath(string configPath, Guid loanGuid, string docType, string attachmentId)
        {
            var folder = Path.Combine(configPath, loanGuid.ToString());
            var filename = string.Format("{0}_{1}", docType, attachmentId);
            var folderWithFile = Path.Combine(folder, filename);

            var directory = Path.GetDirectoryName(folderWithFile);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return folderWithFile;
        }

        public async Task<HashSet<Guid>> GetPersistedDocumentAsync(Guid loanGuid)
        {
            var downloadedDocuments = await _documentRepository.GetDoucmentByLoanGuidAsync(loanGuid).ConfigureAwait(false);
            var documentsGroupByLoanId = EncompassDocumentHelper.GroupDoucmentByLoanId(downloadedDocuments);

            if (documentsGroupByLoanId.ContainsKey(loanGuid))
            {
                return documentsGroupByLoanId[loanGuid];
            }
            return new HashSet<Guid>();
        }


        private async Task<bool> SetDocumentCacheAsync(Guid loanGuid, HashSet<Guid> documents)
        {
            var documentsTxt = JsonConvert.SerializeObject(documents);
            var result = await _redisCacheClient.SetLoanAttachmentAsync(loanGuid, documentsTxt).ConfigureAwait(false);
            return result;
        }

        private HashSet<Guid> GetDocumentIdToDownload(HashSet<Guid> existDocuments, HashSet<Guid> newDocuments)
        {
            if (existDocuments == null || existDocuments.Count == 0)
                return newDocuments;

            newDocuments.ExceptWith(existDocuments);
            return newDocuments;
        }

        private async Task<HashSet<Guid>> GetDocumentCacheAsync(Guid loanGuid)
        {
            var cachingDocuments = await _redisCacheClient.GetAttachmentByLoanGuidAsync(loanGuid).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<HashSet<Guid>>(cachingDocuments);

            return result;
        }

        private Dictionary<Guid, Attachment> ParseDoucmentApiResponse(GetAttachmentResponse response)
        {
            var documentIds = new Dictionary<Guid, Attachment>();
            foreach (var attachment in response.SuccessResponse)
            {
                documentIds.Add(EncompassHelper.GetGuid(attachment.AttachmentId), attachment);
            }
            return documentIds;
        }
    }
}
