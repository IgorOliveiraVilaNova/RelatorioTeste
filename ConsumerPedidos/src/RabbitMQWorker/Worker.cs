using ConsumerPedidos.src.RabbitMQWorker.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
public class Worker : BackgroundService
{

    private readonly ILeituraMensagem _leituraMensagem;
    private readonly string _filaPedidos = "pedidos";
    private readonly ILogger<Worker> _logger;

    public Worker(ILeituraMensagem leituraMensagem, ILogger<Worker> logger)
    {
        _leituraMensagem = leituraMensagem;
        _logger = logger;
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
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation(" [x] Received {0}", message);

                try
                {
                    _leituraMensagem.LerMensagem(message);
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
