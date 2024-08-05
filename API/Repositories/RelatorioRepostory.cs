using API.Models;
using API.Services;

namespace API.Repositories
{
    public class RelatorioRepository : IRelatorioRepository
    {        
        private readonly IMongoService _mongoService;

        public RelatorioRepository(IMongoService mongoService)
        {
            _mongoService = mongoService;
        }

        public async Task<List<Pedido>> ObterListaDePedidosPorCliente(int codigoCliente)
        {
            var result = await _mongoService.ObterListaDePedidosPorCliente(codigoCliente);
            return result;
        }

        public async Task<List<PedidoPorClienteDTO>> ObterQuantidadePedidoPorCliente()
        {
            var result = await _mongoService.ObterQuantidadePedidoPorCliente();
            return result;
        }

        public async Task<double> ObterSomaPrecosAsync(int codigoPedido)
        {
            var pedido = await _mongoService.ObterPedido(codigoPedido);

            if (pedido == null)
            {
                throw new Exception($"Pedido com código {codigoPedido} não encontrado.");
            }

            return pedido.Itens.Sum(item => item.Preco * item.Quantidade);
        }
    }
}
