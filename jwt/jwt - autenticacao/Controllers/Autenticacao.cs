using jwt_autenticacao.Application.autenticacao;
using jwt_autenticacao.Application.Cache;
using jwt_autenticacao.Application.EnviarEmail;
using jwt_autenticacao.Application.Eventos;
using Jwt_autenticacao.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace jwt___autenticacao.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Autenticacao : ControllerBase
    {
        private readonly PessoaContext _context;
        private readonly ICache _cache;
        private readonly IRabbitMQProducer _rabbit;
        private readonly IConfiguration _configuration;

        public Autenticacao(PessoaContext context, ICache cache , IRabbitMQProducer rabbit, IConfiguration configuration)
        {
            _context = context;
            _cache = cache;
            _rabbit = rabbit;
            _configuration = configuration;
        }

        //gera o token

        [HttpGet]
        public IActionResult login (autenticacaoRequest request)
        {      
            try
            {
                var aute = new autenticacaoService(_context, _cache, _rabbit);
                var sucesso = aute.autenticacao(request);

                if (string.IsNullOrWhiteSpace(sucesso?.token))
                {
                    return Unauthorized();
                }
                else
                {
                     
                    return Ok(new { token = sucesso});
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }   
        }
        //Envia msg de sucesso se estiver com o bearer token
        [HttpPost]
        [Route("msgsucesso")]
        [Authorize]
        public IActionResult Sucesso ()
        {
            try
            {
                var autenticacao = new autenticacaoService(_context, _cache, _rabbit);
                var sucesso = autenticacao.MsgSucesso();
                return Ok(sucesso);
            }
            catch (Exception ex) { 
            return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("lerFila")]
        public IActionResult lerFila()
        {
            try
              {
                var autenticacao = new EnviarEmailConsumerService(_configuration);
                autenticacao.configurarConexao();
                return Ok(autenticacao);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
