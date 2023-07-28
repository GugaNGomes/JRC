using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jwt_autenticacao.Application.Cache
{
    public class Cache: ICache
    {
        private readonly IDistributedCache _distributedCache;

        public Cache(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public T? Get<T>(string key)
        {
            var cacheByte = _distributedCache.Get(key);
            if (cacheByte == null)
                return default(T);

            var cacheString = Encoding.UTF8.GetString(cacheByte);
            var resposta = JsonConvert.DeserializeObject<T>(cacheString);
            return resposta;
        }

        public void Set<T>(string key, T value, int ttl)
        {
            var cacheString = JsonConvert.SerializeObject(value);
            var cachebytes = Encoding.UTF8.GetBytes(cacheString);

            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(ttl));
            _distributedCache.Set(key, cachebytes, options);

        }
    }
}
