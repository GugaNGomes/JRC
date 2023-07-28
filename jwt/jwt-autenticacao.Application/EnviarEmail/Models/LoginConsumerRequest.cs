using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jwt_autenticacao.Application.EnviarEmail.Models
{
    public class LoginConsumerRequest
    {
        public string? Email { get; set; }

        public string? Assunto { get; set; }

        public string? Texto { get; set; }
    }
}
