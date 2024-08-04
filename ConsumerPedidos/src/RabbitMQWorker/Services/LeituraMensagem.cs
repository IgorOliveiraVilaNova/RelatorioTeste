using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;


namespace ConsumerPedidos.src.RabbitMQWorker.Services
{
    public class LeituraMensagem : ILeituraMensagem
    {
        private readonly ILogger<Worker> _logger;
        private readonly IPedidoService _pedidoService;
        public LeituraMensagem(ILogger<Worker> logger, IPedidoService pedidoService)
        {
            _logger = logger;
            _pedidoService = pedidoService;
        }
        public void LerMensagem(string message)
        {
            // Convertendo o body para BSON e desserializando para um objeto Pedido
            var document = BsonDocument.Parse(message);
            Pedido pedido = BsonSerializer.Deserialize<Pedido>(document);

            var pedidoValidator = new PedidoValidator();
            ValidationResult result = pedidoValidator.Validate(pedido);

            if (!result.IsValid)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(t => t.ErrorMessage)));
            }

            _pedidoService.InserirPedidoNoMongoDB(pedido);

            _logger.LogInformation(" [x] Finished {0}", message);
        }
    }
}
