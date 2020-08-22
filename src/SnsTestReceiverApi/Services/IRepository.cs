using System.Collections.Generic;
using SnsTestReceiverApi.Models;

namespace SnsTestReceiverApi.Services
{
    public interface IRepository
    {
        IReadOnlyList<string> Search(string keyword, int limit);
        bool TryCreate(string key, SnsMessage value);
        SnsMessage Get(string key);
        bool Delete(string key);
    }
}