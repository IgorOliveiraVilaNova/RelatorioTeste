using ConsumerPedidos.src.RabbitMQWorker.Services;
using MongoDB.Driver;

public class PedidoService : IPedidoService
{
    private readonly IMongoCollection<Pedido> _pedidos;

    public PedidoService()
    {
        var mongoDbHost = Environment.GetEnvironmentVariable("MONGODB_HOST") ?? "localhost";
        var mongoDbPort = Environment.GetEnvironmentVariable("MONGODB_PORT") ?? "27017";
        var connectionString = $"mongodb://{mongoDbHost}:{mongoDbPort}";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("ProjetoBTG");
        _pedidos = database.GetCollection<Pedido>("pedidos");
    }

    public void InserirPedidoNoMongoDB(Pedido pedido)
    {
        _pedidos.InsertOne(pedido);
    }
}
