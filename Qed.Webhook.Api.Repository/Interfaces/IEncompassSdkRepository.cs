using System.Threading.Tasks;
using Qed.Webhook.Api.Repository.Entities;

namespace Qed.Webhook.Api.Repository.Interfaces
{
    public interface IEncompassSdkRepository
    {
        Task<EncompassSdkConfigEntity> GetEncomopassCredentialAsync(string name);
    }
}