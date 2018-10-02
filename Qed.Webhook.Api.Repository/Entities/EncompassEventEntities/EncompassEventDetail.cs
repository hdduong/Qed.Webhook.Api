using System;

namespace Qed.Webhook.Api.Repository.Entities.EncompassEventEntities
{
    public class EncompassEventDetail
    {
        public int Id { get; set; }
        public Guid EventId { get; set; }
        public DateTime EventUtcDtTm { get; set; }
        public int EventTypeId { get; set; }
        public string InstanceId { get; set; }
        public string UserId { get; set; }
        public int ResourceTypeId { get; set; }
        public Guid ResourceId { get; set; }
        public string MsgTxt { get; set; }
    }
}
