using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Microsoft.CodeAnalysis.Text;
using RabbitMQ.Client.Events;
using System.Collections;
using Newtonsoft.Json;
using jwt_autenticacao.Application.EnviarEmail.Models;

namespace jwt_autenticacao.Application.EnviarEmail
{
    public class EnviarEmailConsumerService
    {


        private readonly IConfiguration _configuration;

        public EnviarEmailConsumerService(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public void configurarConexao ()
        {
            var _server = _configuration.GetValue<string>("Rabbit:Server");
            var _userName = _configuration.GetValue<string>("Rabbit:UserName");
            var _password = _configuration.GetValue<string>("Rabbit:Password");
            var _queue = _configuration.GetValue<string>("Rabbit:Queue");
            var _exchange = _configuration.GetValue<string>("Rabbit:Exchange");
            var _routingKey = _configuration.GetValue<string>("Rabbit:RoutingKey");

            var factory = new ConnectionFactory()
            {
                HostName = _server,
                UserName = _userName,
                Password = _password,
            };

            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(_queue, true, false, true);
            channel.ExchangeDeclare(_exchange, "topic", true, true);
            channel.QueueBind(_queue, _exchange, _routingKey);

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: _queue, autoAck: true, consumer: consumer);

            consumer.Received += consumirMensagem;
            Console.ReadKey();

        }

        public void enviarEmail (string? nomeRemetente, string emailDestinatario, string? assunto, string? conteudo)
        {
            
            var porta = 587;
            var smtp = "smtp.titan.email";
            var isSSl = false;
            var usuario = "gustavo.nascimento@varsolutions.com.br";
            var senha = "Var@1234";

            var objEmail = new MailMessage(usuario, emailDestinatario,assunto, conteudo);
            objEmail.From = new MailAddress(nomeRemetente + "<" + usuario + ">");
            objEmail.IsBodyHtml = true;
            objEmail.SubjectEncoding = Encoding.GetEncoding("UTF-8");
            objEmail.BodyEncoding = Encoding.GetEncoding("UTF-8");
            objEmail.Subject = assunto;
            objEmail.Body = conteudo;

            using (var objSmtp = new SmtpClient(smtp, porta))
            {
                objSmtp.EnableSsl = isSSl;
                objSmtp.UseDefaultCredentials = false;
                objSmtp.Credentials = new NetworkCredential(usuario, senha);
                objSmtp.Send(objEmail);
                objEmail.Dispose();
            }
        }

         void consumirMensagem(object? sender, BasicDeliverEventArgs e)
        {
            var eventosString = Encoding.UTF8.GetString(e.Body.ToArray());
            var evento = JsonConvert.DeserializeObject<LoginConsumerRequest>(eventosString);
            if (evento?.Email != null){
                 enviarEmail("Gerenciamento de logins", evento.Email, evento?.Assunto, evento?.Texto);
            }
        }
    }
}
