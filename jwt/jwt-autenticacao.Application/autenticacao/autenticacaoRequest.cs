using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jwt_autenticacao.Application.autenticacao
{
    public class autenticacaoRequest
    {
        public string? userName { get; set; }
        public string? password { get; set; }
    }
}
