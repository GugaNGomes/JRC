using jwt_autenticacao.Application.Cache;
using jwt_autenticacao.Application.EnviarEmail;
using jwt_autenticacao.Application.Eventos;
using jwt_autenticacao.Application.Eventos.Models;
using Jwt_autenticacao.Repository;
using Jwt_autenticacao.Repository.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace jwt_autenticacao.Application.autenticacao
{
    public class autenticacaoService 
    {
        private readonly PessoaContext _context;
        private readonly ICache _cache;
        private readonly IRabbitMQProducer _rabbit;
        public autenticacaoService(PessoaContext context, ICache cache, IRabbitMQProducer rabbit)
        {
            _context = context;
            _cache = cache;
            _rabbit = rabbit;
        }
        public autenticacaoResponse? autenticacao (autenticacaoRequest request)
        {
            var chave = $"{request.userName}{request.password}";

            var usuarioExisteCache = _cache.Get<autenticacaoResponse>(chave);
            if (usuarioExisteCache != null )
            {
                return usuarioExisteCache;
            }
            else
            {
                var verificando = _context?.Usuarios?.FirstOrDefault(x => x.usuario == request.userName && x.senha == request.password);
                if (verificando != null)
                {

                    var TokenString = GerarTokenJwt(verificando);
                    _cache.Set(chave, TokenString, 60);

                    var loginEfetuado = new loginEfetuado()
                    {
                        Email = verificando.email,
                        Assunto = "Login Efetuado",
                        Texto = $"{request.userName}, você acabou de fazer login. Ficamos feliz por retornar."

                    };

                    _rabbit.EnviarMsg(loginEfetuado, "RetornoLogin.email", "Retornologin", "Email");
                    var resposta = new autenticacaoResponse()
                    {
                        token = TokenString,
                        idUsuario = verificando.id
                    };
                    _cache.Set(chave, resposta, 60);
                    return resposta;
                }
                else
                {
                    return null;
                }

            }
        }

        public string MsgSucesso()
        {
            return "Sucesso";
            
        }

        private string GerarTokenJwt(TabUsuario usuario)
        {
            var issuer = "var";
            var audience = "var";
            var key = "5ea4cfa4-badf-4763-ac0e-3bd48fca4c76";


            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
{
                 new Claim("usuarioId", usuario.id.ToString())

            };
            var token = new JwtSecurityToken(issuer: issuer, claims: claims, audience: audience, expires: DateTime.Now.AddMinutes(60), signingCredentials: credentials);
            var TokenHandler = new JwtSecurityTokenHandler();
            var stringToken = TokenHandler.WriteToken(token);
            return stringToken;

        }
    }
}