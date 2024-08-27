using System.Collections.Generic;

namespace Persistence.Repositories
{
    public interface ICacheService
    {
        List<T>? Get<T>(string key);
        void Add(string key, object data);
        void Remove(string key);
        void Clear();
        bool Any(string key);
        bool CheckActive();
    }
}