using SnsTestReceiver.Sdk.Models;
using System.Collections.Concurrent;

namespace SnsTestReceiver.Api.Services
{
    public class InMemoryRepository : IRepository
    {
        private readonly ConcurrentDictionary<string, SnsMessage> _storage = new ConcurrentDictionary<string, SnsMessage>();

        public IReadOnlyList<SnsMessage> Search(string keyword, int limit)
        {
            var query = _storage.Values.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(m => m.Message.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            return query.Take(limit).ToList();
        }

        public bool TryCreate(string key, SnsMessage value)
        {
            return _storage.TryAdd(key, value);
        }

        public SnsMessage Get(string key)
        {
            _storage.TryGetValue(key, out var value);

            return value;
        }

        public bool Delete(string key)
        {
            return _storage.TryRemove(key, out var _);
        }
    }
}