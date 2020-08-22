using SnsTestReceiverApi.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SnsTestReceiverApi.Services
{
    public class InMemoryRepository : IRepository
    {
        private readonly ConcurrentDictionary<string, SnsMessage> _storage = new ConcurrentDictionary<string, SnsMessage>();
        
        public IReadOnlyList<string> Search(string keyword, int limit)
        {
            var query = _storage.Values.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(m => m.Message != null && m.Message.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            return query.Select(m=>m.MessageId).Take(limit).ToList();
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