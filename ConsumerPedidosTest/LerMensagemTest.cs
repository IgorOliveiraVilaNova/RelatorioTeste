using ConsumerPedidos.src.RabbitMQWorker.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ConsumerPedidosTest
{
    public class LerMensagemTest
    {

        private readonly Mock<ILogger<Worker>> _loggerMock;
        private readonly Mock<IPedidoService> _pedidoServiceMock;
        private readonly LeituraMensagem _leituraMensagem;

        public LerMensagemTest()
        {
            _loggerMock = new Mock<ILogger<Worker>>();
            _pedidoServiceMock = new Mock<IPedidoService>();

            _leituraMensagem = new LeituraMensagem(_loggerMock.Object, _pedidoServiceMock.Object);
        }

        [Fact]
        public void Quando_Mensagem_Correta_Deve_Processar() 
        {
            string mensagem = "{ \"codigoPedido\": 1001, \"codigoCliente\": 1, \"itens\": [{ \"produto\": \"lápis\", \"quantidade\": 100, \"preco\": 1.10 }, { \"produto\": \"caderno\", \"quantidade\": 10, \"preco\": 1.00 }] }";

            _leituraMensagem.LerMensagem(mensagem);

            _pedidoServiceMock.Verify(x => x.InserirPedidoNoMongoDB(It.IsAny<Pedido>()), Times.Once());
        }

        [Fact]
        public void Quando_Mensagem_Invalida_Deve_Dar_Exception() 
        {
            string mensagem = "Invalid JSON";
            var exception = Assert.Throws<FormatException>(() => _leituraMensagem.LerMensagem(mensagem));

            Assert.Equal("A mensagem enviada está fora do padrão esperado", exception.Message);
        }

        [Fact]
        public void Quando_Mensagem_Estiver_Sem_Item_Deve_Dar_Exception() 
        {
            string mensagem = "{ \"codigoPedido\": 1001, \"codigoCliente\": 1, \"itens\": [] }";
            
            var exception = Assert.Throws<Exception>(() => _leituraMensagem.LerMensagem(mensagem));

            Assert.Equal("Houve um erro no formato da mensagem. Segue mensagem de erro: A lista de itens não pode ser vazia", exception.Message);
        }

        [Fact]
        public void Quando_Mensagem_Estiver_Com_Item_Null_Deve_Dar_Exception()
        {
            string mensagem = "{ \"codigoPedido\": 1001, \"codigoCliente\": 1, \"itens\": null }";

            var exception = Assert.Throws<Exception>(() => _leituraMensagem.LerMensagem(mensagem));

            Assert.Equal("Houve um erro no formato da mensagem. Segue mensagem de erro: A lista de itens não pode ser vazia", exception.Message);
        }

        [Fact]
        public void Quando_Mensagem_Estiver_Sem_Codigo_Pedido_Valido_Deve_Dar_Exception()
        {
            string mensagem = "{ \"codigoPedido\": 0, \"codigoCliente\": 1, \"itens\": [{ \"produto\": \"lápis\", \"quantidade\": 100, \"preco\": 1.10 }, { \"produto\": \"caderno\", \"quantidade\": 10, \"preco\": 1.00 }] }";

            var exception = Assert.Throws<Exception>(() => _leituraMensagem.LerMensagem(mensagem));

            Assert.Equal("Houve um erro no formato da mensagem. Segue mensagem de erro: O código do pedido não pode ser zero", exception.Message);
        }

        [Fact]
        public void Quando_Mensagem_Estiver_Sem_Codigo_Cliente_Valido_Deve_Dar_Exception()
        {
            string mensagem = "{ \"codigoPedido\": 1001, \"codigoCliente\": 0, \"itens\": [{ \"produto\": \"lápis\", \"quantidade\": 100, \"preco\": 1.10 }, { \"produto\": \"caderno\", \"quantidade\": 10, \"preco\": 1.00 }] }";

            var exception = Assert.Throws<Exception>(() => _leituraMensagem.LerMensagem(mensagem));

            Assert.Equal("Houve um erro no formato da mensagem. Segue mensagem de erro: O código do cliente não pode ser zero", exception.Message);
        }

        [Fact]
        public void Quando_Mensagem_Estiver_Sem_Nome_De_Produto_Deve_Dar_Exception()
        {
            string mensagem = "{ \"codigoPedido\": 1001, \"codigoCliente\": 1, \"itens\": [{ \"produto\": \"\", \"quantidade\": 100, \"preco\": 1.10 }] }";

            var exception = Assert.Throws<Exception>(() => _leituraMensagem.LerMensagem(mensagem));

            Assert.Equal("Houve um erro no formato da mensagem. Segue mensagem de erro: O nome do produto não pode ser vazio", exception.Message);
        }

        [Fact]
        public void Quando_Mensagem_Estiver_Com_Nome_De_Produto_Longo_Deve_Dar_Exception()
        {
            string mensagem = "{ \"codigoPedido\": 1001, \"codigoCliente\": 1, \"itens\": [{ \"produto\": \"O nome desse produto é longo mesmo, mas é pra testar o limite de caracteres que implementamos no ItemValidator.\", \"quantidade\": 100, \"preco\": 1.10 }] }";

            var exception = Assert.Throws<Exception>(() => _leituraMensagem.LerMensagem(mensagem));

            Assert.Equal("Houve um erro no formato da mensagem. Segue mensagem de erro: O nome do produto deve ter entre 1 e 100 caracteres", exception.Message);
        }

        [Fact]
        public void Quando_Mensagem_Estiver_Com_Quantidade_Zero_Deve_Dar_Exception()
        {
            string mensagem = "{ \"codigoPedido\": 1001, \"codigoCliente\": 1, \"itens\": [{ \"produto\": \"lápis\", \"quantidade\": 0, \"preco\": 1.10 }] }";

            var exception = Assert.Throws<Exception>(() => _leituraMensagem.LerMensagem(mensagem));

            Assert.Equal("Houve um erro no formato da mensagem. Segue mensagem de erro: A quantidade de produtos no item do pedido não pode ser menor do que zero", exception.Message);
        }

        [Fact]
        public void Quando_Mensagem_Estiver_Com_Preco_Zero_Deve_Dar_Exception()
        {
            string mensagem = "{ \"codigoPedido\": 1001, \"codigoCliente\": 1, \"itens\": [{ \"produto\": \"lápis\", \"quantidade\": 100, \"preco\": 0 }] }";

            var exception = Assert.Throws<Exception>(() => _leituraMensagem.LerMensagem(mensagem));

            Assert.Equal("Houve um erro no formato da mensagem. Segue mensagem de erro: O preço no item do pedido não pode ser menor do que zero", exception.Message);
        }
    }
}
