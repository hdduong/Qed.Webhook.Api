using Qed.Webhook.Service.Enums;

namespace Qed.Webhook.Service.Models.Responses.Subcription
{
    public class SubscriptionAction
    {
        public SubscriptionActionEnum PerformingAction { get; set; }
        public string SubcriptionId { get; set; }

        public SubscriptionAction()
        {
            PerformingAction = SubscriptionActionEnum.Nothing;
            SubcriptionId = string.Empty;
        }
    }
}
