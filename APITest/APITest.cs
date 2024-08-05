using API.Models;
using API.Repositories;
using API.Services;
using Moq;

namespace APITest
{
    public class APITest
    {
        private readonly RelatorioRepository _repository;
        private readonly Mock<IMongoService> _mongoService;
        private readonly List<Pedido> _pedidosEmMemoria;

        public APITest()
        {
            _pedidosEmMemoria = new List<Pedido>
            {
                new Pedido
                {
                    CodigoPedido = 1001,
                    CodigoCliente = 1,
                    Itens = new List<Item>
                    {
                        new Item { Produto = "Caderno", Quantidade = 2, Preco = 10.5 },
                        new Item { Produto = "Caixa de Canetas", Quantidade = 1, Preco = 20 }
                    }
                },
                new Pedido
                {
                    CodigoPedido = 1002,
                    CodigoCliente = 2,
                    Itens = new List<Item>
                    {
                        new Item { Produto = "Livro", Quantidade = 5, Preco = 15 },
                        new Item { Produto = "Sulfite", Quantidade = 3, Preco = 7.5 }
                    }
                }
            };
            _mongoService = new Mock<IMongoService>();

            _mongoService.Setup(service => service.ObterListaDePedidosPorCliente(It.IsAny<int>()))
               .ReturnsAsync((int codigoCliente) => _pedidosEmMemoria.Where(p => p.CodigoCliente == codigoCliente).ToList());


            _mongoService.Setup(service => service.ObterQuantidadePedidoPorCliente())
                .ReturnsAsync(_pedidosEmMemoria
                    .GroupBy(p => p.CodigoCliente)
                    .Select(g => new PedidoPorClienteDTO
                    {
                        codigoCliente = g.Key,
                        quantidadePedidos = g.Count()
                    })
                    .ToList());

            _mongoService.Setup(service => service.ObterPedido(It.IsAny<int>()))
                .ReturnsAsync((int codigoPedido) => _pedidosEmMemoria.FirstOrDefault(p => p.CodigoPedido == codigoPedido));

            _repository = new RelatorioRepository(_mongoService.Object);
        }

        [Fact]
        public async Task Quando_Codigo_Pedido_Nao_Existe_BD_Deve_Retornar_Erro() 
        {
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _repository.ObterSomaPrecosAsync(313131));

            Assert.Equal("Pedido com código 313131 não encontrado.", actual: exception.Message);
        }

        [Fact]
        public async Task Quando_Codigo_Existir_No_BD_Retornar_Soma_Produtos() 
        {
            var soma = await _repository.ObterSomaPrecosAsync(1001);
            Assert.Equal(41, soma);
        }
    }
}
