using API.Repositories;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var mongoDbHost = Environment.GetEnvironmentVariable("MONGODB_HOST") ?? "localhost";
var mongoDbPort = Environment.GetEnvironmentVariable("MONGODB_PORT") ?? "27017";
var connectionString = $"mongodb://{mongoDbHost}:{mongoDbPort}";

var mongoClient = new MongoClient(connectionString);
var database = mongoClient.GetDatabase("ProjetoBTG");

builder.Services.AddSingleton<IMongoDatabase>(database);
builder.Services.AddScoped<IRelatorioRepository, RelatorioRepostory>();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/pedido/soma/{codigoPedido}", async (int codigoPedido, IRelatorioRepository relatorioRepository) =>
{
	try
	{
		var somaPrecos = await relatorioRepository.ObterSomaPrecosAsync(codigoPedido);
		if (somaPrecos == null)
		{
			return Results.NotFound();
		}

		return Results.Ok(new { codigoPedido, somaPrecos });
	}
	catch (Exception ex)
	{
        return Results.Problem(statusCode: 500, detail: ex.Message);
    }
});

app.MapGet("/pedido/", async(IRelatorioRepository relatorioRepository) => 
{
	try
	{
		var pedidosPorCliente = await relatorioRepository.ObterQuantidadePedidoPorCliente();
		if (pedidosPorCliente == null)
		{
			return Results.NotFound();
		}
		return Results.Ok(pedidosPorCliente);
	}
	catch (Exception ex)
	{
        return Results.Problem(statusCode: 500, detail: ex.Message);        
	}
});

app.MapGet("/pedido/{codigoCliente}", async (int codigoCliente, IRelatorioRepository relatorioRepository) => 
{
	try
	{
		var listaPedidos = relatorioRepository.ObterListaDePedidosPorCliente(codigoCliente);
		return Results.Ok(listaPedidos);
	}
	catch (Exception ex)
	{
		return Results.Problem(statusCode: 500, detail: ex.Message);
	}
});

app.Run();
