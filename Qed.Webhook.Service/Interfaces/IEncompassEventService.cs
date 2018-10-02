using System.Threading.Tasks;
using Qed.Webhook.Service.Models.Requests.Event;
using Qed.Webhook.Service.Models.Responses.Event;

namespace Qed.Webhook.Service.Interfaces
{
    public interface IEncompassEventService
    {
        Task<AddEncompassEventResponse> AddEventToDbAsync(AddEncompassEventRequest request);
    }
}