using System;
using System.Collections.Generic;
using System.Linq;
using Qed.Webhook.Service.Enums;
using Qed.Webhook.Service.Models.Requests.Subcription;
using Qed.Webhook.Service.Models.Responses.Subcription;

namespace Qed.Webhook.Service.Helpers
{
    public static class EncompassSubscriptionHelper
    {
        public static SubscriptionAction GetSubscriptionAction(this EncompassSubscription configSubscription,
            List<SubscriptionSuccessResponse> existingSubscriptions)
        {
            var performingAction = new SubscriptionAction();

            var exsitingResourceSubscription = existingSubscriptions.FirstOrDefault(x => x.Resource.Equals(configSubscription.Resource, StringComparison.CurrentCultureIgnoreCase));

            // same resource was subscribied
            if (exsitingResourceSubscription != null)
            {
                // in encompass combination of resourceType and endpoint is unique key
                var existingResourceEndpointSubcription = existingSubscriptions.FirstOrDefault(x =>
                    x.Resource.Equals(configSubscription.Resource, StringComparison.CurrentCultureIgnoreCase) && x.Endpoint.Equals(configSubscription.Endpoint,
                        StringComparison.CurrentCultureIgnoreCase));

                if (existingResourceEndpointSubcription != null)
                {
                    var missingConfigEvent = configSubscription.Events.Except(existingResourceEndpointSubcription.Events).ToList();
                    if (missingConfigEvent.Count > 0)
                    {
                        performingAction.PerformingAction = SubscriptionActionEnum.Update;
                        performingAction.SubcriptionId = existingResourceEndpointSubcription.SubscriptionId.ToString();
                    }
                }
                else
                {
                    // resource is there but we need update endpoint
                    performingAction.PerformingAction = SubscriptionActionEnum.Update;
                    performingAction.SubcriptionId = exsitingResourceSubscription.SubscriptionId.ToString();
                }
               
            }
            else
            {
                performingAction.PerformingAction = SubscriptionActionEnum.Add;
            }

            return performingAction;
        }
    }
}
