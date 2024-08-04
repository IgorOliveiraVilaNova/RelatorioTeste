using ConsumerPedidos.src.RabbitMQWorker.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace ConsumerPedidosTest
{
    public class LerMensagemTest
    {

        private readonly Mock<ILogger<Worker>> _loggerMock;
        private readonly Mock<IPedidoService> _pedidoServiceMock;
        private readonly LeituraMensagem _leituraMensagemMock;

        public LerMensagemTest()
        {
            _loggerMock = new Mock<ILogger<Worker>>();
            _pedidoServiceMock = new Mock<IPedidoService>();

            _leituraMensagemMock = new LeituraMensagem(_loggerMock.Object, _pedidoServiceMock.Object);
        }

        [Fact]
        public void Quando_Mensagem_Correta_Deve_Processar() 
        {
            string mensagem = "{ \"codigoPedido\": 1001, \"codigoCliente\": 1, \"itens\": [{ \"produto\": \"lápis\", \"quantidade\": 100, \"preco\": 1.10 }, { \"produto\": \"caderno\", \"quantidade\": 10, \"preco\": 1.00 }] }";

            _leituraMensagemMock.LerMensagem(mensagem);

            _pedidoServiceMock.Verify(x => x.InserirPedidoNoMongoDB(It.IsAny<Pedido>()), Times.Once());
        }
    }
}
