using Infrastructure.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace TLD15.Pages.Lore;

public class EntityLore : EntityBase<Guid>, IEntityStored
{
    public static string Collection => "lore";

    [BsonElement("title"), MaxLength(140)]
    public string Title { get; set; } = string.Empty;

    [BsonElement("subtitle"), MaxLength(140)]
    public string SubTitle { get; set; } = string.Empty;

    [BsonElement("poster_url")]
    public string PosterUrl { get; set; } = string.Empty;

    [BsonElement("poster_alt")]
    public string PosterAlt { get; set; } = string.Empty;

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("content_html")]
    public string ContentHtml { get; set; } = string.Empty;


    public static async Task CreateIndexesAsync(IMongoDatabase database)
    {
        var collection = database.GetCollection<EntityLore>(Collection);

        var indexLanguage = new CreateIndexModel<EntityLore>(
            new IndexKeysDefinitionBuilder<EntityLore>().Ascending(new StringFieldDefinition<EntityLore>("language")),
            new CreateIndexOptions() { Unique = true });
        await collection.Indexes.CreateOneAsync(indexLanguage);
    }
}
