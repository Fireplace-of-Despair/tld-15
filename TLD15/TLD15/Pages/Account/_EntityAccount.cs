using Infrastructure.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace TLD15.Pages.Account;

public sealed class EntityAccount : EntityBase<Guid>, IEntityStored
{
    public static string Collection => "accounts";
    public static string Database => "core";

    [BsonElement("login"), MaxLength(100)]
    public required string Login { get; set; }

    [BsonElement("password")]
    public string Password { get; set; } = string.Empty;

    [BsonElement("salt")]
    public string Salt { get; set; } = string.Empty;

    public static async Task CreateIndexesAsync(IMongoClient client)
    {
        var database = client.GetDatabase(Database);
        var collection = database.GetCollection<EntityAccount>(Collection);

        var field = new StringFieldDefinition<EntityAccount>("login");
        var indexDefinition = new IndexKeysDefinitionBuilder<EntityAccount>().Ascending(field);

        var indexModel = new CreateIndexModel<EntityAccount>(indexDefinition, new CreateIndexOptions() { Unique = true });

        await collection.Indexes.CreateOneAsync(indexModel);
    }
}
