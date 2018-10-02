using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using Qed.Webhook.Api.Repository.Entities;
using Qed.Webhook.Api.Repository.Interfaces;
using LoanDocumentEnitty = Qed.Webhook.Api.Shared.Models.LoanDocumentEnitty;

namespace Qed.Webhook.Api.Repository.Repositories
{
    public class EncompassDocumentRepository : IEncompassDocumentRepository
    {
        private readonly IDbConnectionFactory _dbConnection;

        public EncompassDocumentRepository(IDbConnectionFactory dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<LoanDocumentEnitty>> GetDoucmentByLoanGuidAsync(Guid loanGuid)
        {
            var result = new List<LoanDocumentEnitty>();
            using (IDbConnection connection = _dbConnection.CreateStagingConnection())
            {
                var documents = await connection.QueryAsync<LoanDocumentEnitty>(
                    @"	select
	                    edd.LoanGuid
	                    , edd.DocumentGuid
	                    , max(edd.EndDownloadDtTm) AS 'LatestDownloadDtTm'
	                    from dbo.EncompassWebhookDocumentDownload edd
	                    where edd.DownloadSucceededBit = 1
	                    and edd.LoanGuid = @LoanGuid
	                    group by edd.DocumentGuid, edd.LoanGuid", new
                    {
                        LoanGuid = loanGuid
                    }, commandType: CommandType.Text).ConfigureAwait(false);

                result = documents.ToList();
            }

            return result;
        }

        public async Task<DbResponse<EncompassDocumentEntity>> AddDocumentAsync(EncompassDocumentEntity addingDocument)
        {
            var addDocumentDbResponse =
                new DbResponse<EncompassDocumentEntity>(new EncompassDocumentEntity()) {Body = addingDocument};

            try
            {
                int newId;
                using (IDbConnection connection = _dbConnection.CreateStagingConnection())
                {
                    var ids = await connection.QueryAsync<int>(
                        "[dbo].[AddDownloadDocument]", new
                        {
                            addingDocument.LoanGuid,
                            addingDocument.DocumentTitle,
                            addingDocument.DocumentTypeName,
                            addingDocument.DocumentDtTm,
                            addingDocument.DocumentGuid,
                            addingDocument.AddedById,
                            addingDocument.AddedByName,
                            addingDocument.FileName,
                            addingDocument.FileSizeBytes,
                            addingDocument.DownloadSucceededBit,
                            addingDocument.DownloadFileFullPath,
                            addingDocument.StartDownloadDtTm,
                            addingDocument.EndDownloadDtTm,
                            addingDocument.IsActiveBit,
                            addingDocument.DownloadStatusTypeId,
                            addingDocument.ErrorMsgTxt
                        }, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                    newId = ids.FirstOrDefault();
                }

                addDocumentDbResponse.IsSuccessBit = true;
                addDocumentDbResponse.Body.Id = newId;
                
            }
            catch (Exception ex)
            {
                addDocumentDbResponse.IsSuccessBit = false;
                addDocumentDbResponse.ErrorMsgTxt = JsonConvert.SerializeObject(new { message = JsonConvert.SerializeObject(ex.Message), stackTrace = ex.StackTrace });
            }
            return addDocumentDbResponse;
        }

        public async Task<DbResponse<EncompassDocumentEntity>> UpdateDocumentAsync(EncompassDocumentEntity updatingDocument)
        {
            var updateDocumentDbResponse =
                new DbResponse<EncompassDocumentEntity>(new EncompassDocumentEntity()) { Body = updatingDocument };

            try
            {
                using (IDbConnection connection = _dbConnection.CreateStagingConnection())
                {
                    var rowAffected = await connection.ExecuteAsync(
                        "[dbo].[UpdateDownloadDocument]", new
                        {
                            updatingDocument.Id,
                            updatingDocument.LoanGuid,
                            updatingDocument.DocumentTitle,
                            updatingDocument.DocumentTypeName,
                            updatingDocument.DocumentDtTm,
                            updatingDocument.DocumentGuid,
                            updatingDocument.AddedById,
                            updatingDocument.AddedByName,
                            updatingDocument.FileName,
                            updatingDocument.FileSizeBytes,
                            updatingDocument.DownloadSucceededBit,
                            updatingDocument.DownloadFileFullPath,
                            updatingDocument.StartDownloadDtTm,
                            updatingDocument.EndDownloadDtTm,
                            updatingDocument.IsActiveBit,
                            updatingDocument.DownloadStatusTypeId,
                            updatingDocument.ErrorMsgTxt
                        }, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                }

                updateDocumentDbResponse.IsSuccessBit = true;
            }
            catch (Exception ex)
            {
                updateDocumentDbResponse.IsSuccessBit = false;
                updateDocumentDbResponse.ErrorMsgTxt = JsonConvert.SerializeObject(new { message = JsonConvert.SerializeObject(ex.Message), stackTrace = ex.StackTrace });
            }

            return updateDocumentDbResponse;
        }

    }
}
