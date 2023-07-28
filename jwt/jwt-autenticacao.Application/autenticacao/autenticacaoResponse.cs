using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace jwt_autenticacao.Application.autenticacao
{
    public class autenticacaoResponse
    {
        public string? token { get; set; }
        public int idUsuario { get; set;}
    }
}
