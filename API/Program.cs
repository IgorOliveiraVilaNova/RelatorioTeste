using API.Repositories;
using API.Services;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var mongoDbHost = Environment.GetEnvironmentVariable("MONGODB_HOST") ?? "localhost";
var mongoDbPort = Environment.GetEnvironmentVariable("MONGODB_PORT") ?? "27017";
var connectionString = $"mongodb://{mongoDbHost}:{mongoDbPort}";

var mongoClient = new MongoClient(connectionString);
var database = mongoClient.GetDatabase("ProjetoBTG");

builder.Services.AddSingleton<IMongoDatabase>(database);
builder.Services.AddScoped<IMongoService, MongoService>();
builder.Services.AddScoped<IRelatorioRepository, RelatorioRepository>();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/pedido/totalizadorporpedido/{codigoPedido}", async (int codigoPedido, IRelatorioRepository relatorioRepository) =>
{
	try
	{
		var somaPrecos = await relatorioRepository.ObterSomaPrecosAsync(codigoPedido);
		if (somaPrecos == null)
		{
			return Results.NotFound();
		}

		var valorFormatado = somaPrecos.ToString("F");

        return Results.Ok(new { codigoPedido, valorFormatado});
	}
	catch (Exception ex)
	{
        return Results.Problem(statusCode: 500, detail: ex.Message);
    }
});

app.MapGet("/pedido/relatoriopedidoporcliente", async(IRelatorioRepository relatorioRepository) => 
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

app.MapGet("/pedido/listapedidosporcliente/{codigoCliente}", async (int codigoCliente, IRelatorioRepository relatorioRepository) => 
{
	try
	{
		var listaPedidos = await relatorioRepository.ObterListaDePedidosPorCliente(codigoCliente);
		return Results.Ok(listaPedidos);
	}
	catch (Exception ex)
	{
		return Results.Problem(statusCode: 500, detail: ex.Message);
	}
});

app.Run();
