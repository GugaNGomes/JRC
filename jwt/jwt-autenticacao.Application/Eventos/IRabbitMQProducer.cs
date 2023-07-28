using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jwt_autenticacao.Application.Eventos
{
    public interface IRabbitMQProducer
    {

        void EnviarMsg<T>(T message, string queue, string exchange, string routingKey);
    }
}
