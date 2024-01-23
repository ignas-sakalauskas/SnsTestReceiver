using System.Collections.Generic;
using System.Threading.Tasks;
using SnsTestReceiver.Sdk.Models;

namespace SnsTestReceiver.Sdk
{
    public interface ISnsTestReceiverClient
    {
        Task<IReadOnlyList<SnsMessage>> SearchAsync(string text = null, int? limit = null);
        Task<SnsMessage> GetAsync(string messageId);
        Task DeleteAsync(string messageId);
        Task CreateAsync(string body);
        Task ConfirmSubscriptionAsync();
    }
}
