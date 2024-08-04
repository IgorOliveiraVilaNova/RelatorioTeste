using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client.Events;
using System.Text;

namespace ConsumerPedidosTest
{
    public class WorkerTests
    {
        private readonly Mock<ILogger<Worker>> _loggerMock;
        private readonly Mock<PedidoService> _pedidoServiceMock;
        private readonly Worker _worker;

        public WorkerTests()
        {
            _loggerMock = new Mock<ILogger<Worker>>();
            _pedidoServiceMock = new Mock<PedidoService>();

            _worker = new Worker(_loggerMock.Object, _pedidoServiceMock.Object);
        }

        [Fact]
        public async Task Worker_Recebe_Mensagem_Deve_Processar() 
        {
            //Configuração
            var messageBody = Encoding.UTF8.GetBytes("{ \"codigoPedido\": 1001, \"codigoCliente\": 1, \"itens\": [{ \"produto\": \"lápis\", \"quantidade\": 100, \"preco\": 1.10 }, { \"produto\": \"caderno\", \"quantidade\": 10, \"preco\": 1.00 }] }");
            var mockEvent = new BasicDeliverEventArgs
            {
                Body = new ReadOnlyMemory<byte>(messageBody),
                DeliveryTag = 1
            };

            var consumerMock = new Mock<EventingBasicConsumer>(null);
            consumerMock.Raise(c => c.Received += null, new object[] { null, mockEvent });

            //Ação
            await _worker.StartAsync(CancellationToken.None);

            //Asserção
            _pedidoServiceMock.Verify(x => x.InserirPedidoNoMongoDB(It.IsAny<Pedido>()), Times.Once);
            _loggerMock.Verify(x => x.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);

        }
    }
}
