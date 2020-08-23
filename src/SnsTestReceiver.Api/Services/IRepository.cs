using SnsTestReceiver.Sdk.Models;
using System.Collections.Generic;

namespace SnsTestReceiver.Api.Services
{
    public interface IRepository
    {
        IReadOnlyList<SnsMessage> Search(string keyword, int limit);
        bool TryCreate(string key, SnsMessage value);
        SnsMessage Get(string key);
        bool Delete(string key);
    }
}