using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerPedidos.src.RabbitMQWorker.Services
{
    public interface ILeituraMensagem
    {
        void LerMensagem(string message);
    }
}
