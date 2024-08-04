using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Item
{
    [BsonElement("produto")]
    public string Produto { get; set; }

    [BsonElement("quantidade")]
    public int Quantidade { get; set; }

    [BsonElement("preco")]
    public double Preco { get; set; }
}
