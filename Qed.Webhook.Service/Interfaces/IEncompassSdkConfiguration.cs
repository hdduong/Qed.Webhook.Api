using System.Threading.Tasks;
using Qed.Webhook.Service.Models;

namespace Qed.Webhook.Service.Interfaces
{
    public interface IEncompassSdkConfiguration
    {
        Task<EncompassSdkConfig> GetEncompassSdkConfigAsync();
    }
}