using Infrastructure.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace TLD15.Pages.Introduction;

public sealed class EntityIntroduction : EntityBase<Guid>, IEntityStored
{
    public static string Collection => "introduction";
    public static string Database => "core";

    [BsonElement("poster_url")]
    public string PosterUrl { get; set; } = string.Empty;

    [BsonElement("poster_alt")]
    public string PosterAlt { get; set; } = string.Empty;

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("content_html")]
    public string ContentHtml { get; set; } = string.Empty;

    public static Task CreateIndexesAsync(IMongoClient client)
    {
        return Task.CompletedTask;
    }
}
