using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jwt_autenticacao.Application.Cache
{
    public interface ICache
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, int ttl);

    }
}
