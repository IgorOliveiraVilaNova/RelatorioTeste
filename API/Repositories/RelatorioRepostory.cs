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
            return await _mongoService.ObterListaDePedidosPorCliente(codigoCliente);
        }

        public async Task<List<PedidoPorClienteDTO>> ObterQuantidadePedidoPorCliente()
        {
            return await _mongoService.ObterQuantidadePedidoPorCliente();            
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
