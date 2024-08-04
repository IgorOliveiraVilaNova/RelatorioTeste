using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Pedido
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("codigoPedido")]
    public int CodigoPedido { get; set; }

    [BsonElement("codigoCliente")]
    public int CodigoCliente { get; set; }

    [BsonElement("itens")]
    public List<Item> Itens { get; set; }
}
