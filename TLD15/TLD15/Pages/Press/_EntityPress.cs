using Infrastructure.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace TLD15.Pages.Press;

public sealed class EntityPress : EntityBase<Guid>, IEntityStored
{
    public static string Collection => "press";
    public static string Database => "core";

    [BsonElement("title"), MaxLength(140)]
    public string Title { get; set; } = string.Empty;

    [BsonElement("subtitle"), MaxLength(300)]
    public string Subtitle { get; set; } = string.Empty;

    [BsonElement("poster_url")]
    public string PosterUrl { get; set; } = string.Empty;

    [BsonElement("poster_alt")]
    public string PosterAlt { get; set; } = string.Empty;

    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;

    public static Task CreateIndexesAsync(IMongoClient client)
    {
        return Task.CompletedTask;
    }
}
