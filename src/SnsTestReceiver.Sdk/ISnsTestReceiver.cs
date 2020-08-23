using SnsTestReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnsTestReceiver.Sdk
{
    public interface ISnsTestReceiver
    {
        Task<IReadOnlyList<SnsMessage>> GetAllAsync(string search = null, int? limit = null);
        Task<SnsMessage> GetAsync(string messageId);
        Task DeleteAsync(string messageId);
        Task CreateAsync(string body);
    }
}
