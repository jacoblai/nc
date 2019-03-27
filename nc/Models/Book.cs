using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace BooksApi.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("bookName")]
        public string BookName { get; set; }

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("author")]
        public string Author { get; set; }
        [BsonElement("releaseDate")]
        public DateTime ReleaseDate { get; set; }
    }
}