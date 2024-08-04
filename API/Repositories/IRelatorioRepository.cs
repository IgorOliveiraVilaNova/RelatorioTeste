using API.Models;

namespace API.Repositories
{
    public interface IRelatorioRepository
    {
        Task<double> ObterSomaPrecosAsync(int codigoPedido);
        Task<List<PedidoPorClienteDTO>> ObterQuantidadePedidoPorCliente();
        Task<List<Pedido>> ObterListaDePedidosPorCliente(int codigoCliente);
    }
}
