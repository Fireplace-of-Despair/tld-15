using Infrastructure.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace TLD15.Pages.Social;

public sealed class EntitySocial : EntityBase<Guid>, IEntityStored
{
    public static string Collection => "social";
    public static string Database => "core";

    [BsonElement("name"), MaxLength(140)]
    public string Name { get; set; } = string.Empty;

    [BsonElement("url"), MaxLength(140)]
    public string Url { get; set; } = string.Empty;

    public static Task CreateIndexesAsync(IMongoClient client)
    {
        return Task.CompletedTask;
    }
} 
