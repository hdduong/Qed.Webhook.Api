using System;
using Microsoft.AspNetCore.Mvc;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Api.Shared.Models.WebhookNotification.Redis.Requests;
using Qed.Webhook.Api.Shared.Models.WebhookNotification.Redis.Responses;
using Qed.Webhook.RedisCache.Api.Interfaces;


namespace Qed.Webhook.RedisCache.Api.Controllers
{
    [Produces(ConstantString.JsonContentTypeValue)]
    [Route(ConstantString.RedisCacheUri)]
    public class RedisCacheController : Controller
    {
        private readonly IRedisCacheService _redisCacheService;

        public RedisCacheController(IRedisCacheService redisCacheService)
        {
            _redisCacheService = redisCacheService;
        }

        [HttpGet]
        [Route(ConstantString.RedisCacheWorkerUri)]
        public IActionResult GetAssignningJobWorkerId(Guid resourceId)
        {
            var workerId = _redisCacheService.GetWorkerIdForJob(resourceId);
            return Ok(new GetWorkerIdJobRedisCacheResponse { WorkerId = workerId });
        }

        [HttpPost]
        [Route(ConstantString.RedisCacheWorkerUri)]
        public IActionResult UpdateAssignningJobWorkerId(Guid resourceId, [FromBody]AssignJobWorkerIdRedisCacheRequest request)
        {
            var result = _redisCacheService.SetWorkerIdForJob(resourceId, request.WorkerId);
            return Ok(new AssignJobWokerIdRedisCacheResponse {IsSuccessBit = result});
        }

        [HttpGet]
        [Route(ConstantString.RedisCacheDocumentUri)]
        public IActionResult GetAttachmentByLoanId(Guid resourceId)
        {
            var attachments = _redisCacheService.GetAttachmentForLoan(resourceId);
            return Ok(new GetDocumentRedisCacheResponse { Attachments = attachments});
        }

        [HttpPost]
        [Route(ConstantString.RedisCacheDocumentUri)]
        public IActionResult SetAttachmentForLoanId(Guid resourceId, [FromBody]SetDocumentRedisCacheRequest request)
        {
            var result = _redisCacheService.SetAttachmentForLoan(resourceId, request.Attachment);
            return Ok(new { IsSuccessBit = result });
        }
    }
}