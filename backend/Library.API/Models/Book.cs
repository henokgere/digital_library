using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Library.API.Models;

public class Book
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("author")]
    public string Author { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = "available";

    [BsonElement("borrowedBy")]
    public string? BorrowedBy { get; set; }

    [BsonElement("borrowedDate")]
    public DateTime? BorrowedDate { get; set; }
}
