using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jwt_autenticacao.Application.Eventos
{
    public class RabbitMQProducer : IRabbitMQProducer
    { 
        private readonly string? _server;
        private readonly string? _userName;
        private readonly string? _password;
        private readonly IConfiguration _configuration;

        public RabbitMQProducer(IConfiguration configuration)
        {
            _configuration = configuration;
            _server = _configuration["Rabbit:Server"];
            _userName = _configuration["Rabbit:UserName"];
            _password = _configuration["Rabbit:Password"];

        }

        public void EnviarMsg<T>(T message, string queue, string exchange, string routingKey)
        {

            var factory = new ConnectionFactory()
            {

                HostName = _server,
                UserName = _userName,
                Password = _password,
            };
            
            var conn = factory.CreateConnection();
            using var channel = conn.CreateModel();

            channel.QueueDeclare(queue, true, false, true);
            channel.ExchangeDeclare(exchange, "topic", true, true);
            channel.QueueBind(queue, exchange, routingKey);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: exchange, routingKey: routingKey, body: body);
            
        }
    }
}
