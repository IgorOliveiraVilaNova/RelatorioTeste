using API.Models;

namespace API.Services
{
    public interface IMongoService
    {
        Task<List<Pedido>> ObterListaDePedidosPorCliente(int codigoCliente);
        Task<List<PedidoPorClienteDTO>> ObterQuantidadePedidoPorCliente();
        Task<Pedido> ObterPedido(int codigoPedido);
    }
}
