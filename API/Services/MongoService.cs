using API.Models;
using MongoDB.Driver;

namespace API.Services
{
    public class MongoService : IMongoService
    {
        private readonly IMongoCollection<Pedido> _pedidos;

        public MongoService()
        {
            var mongoDbHost = Environment.GetEnvironmentVariable("MONGODB_HOST") ?? "localhost";
            var mongoDbPort = Environment.GetEnvironmentVariable("MONGODB_PORT") ?? "27017";
            var connectionString = $"mongodb://{mongoDbHost}:{mongoDbPort}";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("ProjetoBTG");
            _pedidos = database.GetCollection<Pedido>("pedidos");
        }

        public async Task<List<Pedido>> ObterListaDePedidosPorCliente(int codigoCliente)
        {
            var filter = Builders<Pedido>.Filter.Eq(p => p.CodigoCliente, codigoCliente);
            return await _pedidos.Find(filter).ToListAsync();
        }

        public async Task<List<PedidoPorClienteDTO>> ObterQuantidadePedidoPorCliente()
        {
            var aggregate = await _pedidos.Aggregate()
                        .Group(p => p.CodigoCliente, g => new PedidoPorClienteDTO
                        {
                            codigoCliente = g.Key,
                            quantidadePedidos = g.Count()
                        })
                        .ToListAsync();

            return aggregate;
        }

        public async Task<Pedido> ObterPedido(int codigoPedido)
        {
            var filter = Builders<Pedido>.Filter.Eq(p => p.CodigoPedido, codigoPedido);
            return await _pedidos.Find(filter).FirstOrDefaultAsync();            
        }
    }
}
