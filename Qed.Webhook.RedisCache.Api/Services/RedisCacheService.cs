using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Qed.Webhook.Api.Repository.Interfaces;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Api.Shared.Helpers;
using Qed.Webhook.RedisCache.Api.Enums;
using Qed.Webhook.RedisCache.Api.Interfaces;

namespace Qed.Webhook.RedisCache.Api.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IRedisCacheHelper _redisCacheHelper;
        private readonly IRedisCacheConfiguration _redisCacheConfiguration;
        private readonly IRedisCacheRepository _redisCacheRepository;

        public RedisCacheService(IRedisCacheHelper redisCacheHelper, IRedisCacheConfiguration redisCacheConfiguration, IRedisCacheRepository redisCacheRepository)
        {
            _redisCacheHelper = redisCacheHelper;
            _redisCacheConfiguration = redisCacheConfiguration;
            _redisCacheRepository = redisCacheRepository;
        }

        public int GetWorkerIdForJob(Guid resourceId)
        {
            var resourceIdKey = resourceId + CacheTypeEnum.Queue.ToString();
            var workerId = _redisCacheHelper.GetCacheItem(resourceIdKey);

            if (string.IsNullOrEmpty(workerId))
            {
                var lastWorkerJob = _redisCacheHelper.GetCacheItem(ConstantString.LastAssignedWorkerId);

                // there is no previous assigned workerId when program starts
                if (string.IsNullOrEmpty(lastWorkerJob))
                {
                    return int.Parse(ConstantString.DefaultWorkerId);
                }

                var lastWorkerId = int.Parse(lastWorkerJob);
                lastWorkerId++;

                if (lastWorkerId > _redisCacheConfiguration.NumberOfWorkers)
                {
                    return int.Parse(ConstantString.DefaultWorkerId);
                }

                return lastWorkerId;
            }

            return int.Parse(workerId);
        }

        public bool SetWorkerIdForJob(Guid resourceId, int workerId)
        {
            var resourceIdKey = resourceId + CacheTypeEnum.Queue.ToString();

            var result = _redisCacheHelper.AddCacheItem(resourceIdKey, workerId.ToString());
            result &= _redisCacheHelper.AddCacheItem(ConstantString.LastAssignedWorkerId, workerId.ToString());

            return result;
        }

        public string GetAttachmentForLoan(Guid resourceId)
        {
            var resourceIdKey = resourceId + CacheTypeEnum.Document.ToString();
            var attachments = _redisCacheHelper.GetCacheItem(resourceIdKey);

            if (string.IsNullOrEmpty(attachments))
                return string.Empty;

            return attachments;
        }

        public bool SetAttachmentForLoan(Guid resourceId, string attachments)
        {
            var resourceIdKey = resourceId + CacheTypeEnum.Document.ToString();
            var result = _redisCacheHelper.AddCacheItem(resourceIdKey, attachments);

            return result;
        }

        public async Task<bool> BootstrapDocumentAsync()
        {
            var addToCacheBit = true;
            var documentsDownloaded = await _redisCacheRepository.LoadDownloadDocument().ConfigureAwait(false);

            var loanDocuments = EncompassDocumentHelper.GroupDoucmentByLoanId(documentsDownloaded);
            foreach (var loanDocument in loanDocuments)
            {
                addToCacheBit &=
                    SetAttachmentForLoan(loanDocument.Key, JsonConvert.SerializeObject(loanDocument.Value));
            }

            return addToCacheBit;
        }

        
    }
}
