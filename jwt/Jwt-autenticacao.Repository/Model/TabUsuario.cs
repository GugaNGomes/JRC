using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jwt_autenticacao.Repository.Model
{
    public class TabUsuario
    {
        
        public int id { get; set; }
        public string? Nome { get; set; }
        public string? usuario { get; set; }
        public string? senha { get; set; }
        public string? email { get; set; }
    }
}
