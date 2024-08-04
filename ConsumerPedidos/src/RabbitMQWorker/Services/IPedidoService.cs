namespace ConsumerPedidos.src.RabbitMQWorker.Services
{
    public interface IPedidoService
    {
        void InserirPedidoNoMongoDB(Pedido pedido);
    }
}
