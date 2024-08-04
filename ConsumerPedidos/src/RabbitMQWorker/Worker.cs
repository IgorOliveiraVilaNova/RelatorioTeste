using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using FluentValidation.Results;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly PedidoService _pedidoService;
    private readonly string _filaPedidos = "pedidos";
    public Worker(ILogger<Worker> logger, PedidoService pedidoService)
    {
        _logger = logger;
        _pedidoService = pedidoService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        var rabbitMqPort = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672");

        var factory = new ConnectionFactory() { HostName = rabbitMqHost, Port = rabbitMqPort };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: $"{_filaPedidos}.dlq",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueDeclare(queue: _filaPedidos,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    _logger.LogInformation(" [x] Received {0}", message);

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
                catch (Exception ex)
                {
                    Console.WriteLine(" [x] Falha ao processar pedido: {0}", ex.Message);

                    // Add error information to the message headers
                    var properties = channel.CreateBasicProperties();
                    properties.Headers = ea.BasicProperties.Headers ?? new Dictionary<string, object>();
                    properties.Headers["Error"] = Encoding.UTF8.GetBytes(ex.Message);
                    properties.Headers["ErrorTimestamp"] = Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("o"));

                    // Publish the message to the DLQ with the added error headers
                    channel.BasicPublish(exchange: "",
                                         routingKey: $"{_filaPedidos}.dlq",
                                         basicProperties: properties,
                                         body: ea.Body);                    
                }                
            };
            channel.BasicConsume(queue: _filaPedidos,
                                 autoAck: true,
                                 consumer: consumer);

            _logger.LogInformation(" [*] Waiting for messages. To exit press CTRL+C");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
