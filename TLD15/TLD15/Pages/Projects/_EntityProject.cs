﻿using Infrastructure.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TLD15.Pages.Articles;

namespace TLD15.Pages.Projects;

public class EntityProject : EntityBase<Guid>, IEntityStored
{
    public static string Collection => "projects";
    public static string Database => "production";

    [BsonElement("id_friendly"), MaxLength(140)]
    public string IdFriendly { get; set; } = string.Empty;

    [BsonElement("title"), MaxLength(140)]
    public string Title { get; set; } = string.Empty;

    [BsonElement("subtitle"), MaxLength(300)]
    public string SubTitle { get; set; } = string.Empty;

    [BsonElement("poster_url")]
    public string PosterUrl { get; set; } = string.Empty;

    [BsonElement("poster_alt")]
    public string PosterAlt { get; set; } = string.Empty;

    [BsonElement("division_code"), MaxLength(3)]
    public string DivisionCode { get; set; } = string.Empty;

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("content_html")]
    public string ContentHtml { get; set; } = string.Empty;

    public Dictionary<string, string> Links { get; set; } = [];

    public static async Task CreateIndexesAsync(IMongoClient client)
    {
        var database = client.GetDatabase(Database);
        var collection = database.GetCollection<EntityArticle>(Collection);

        var fieldIdFriendly = new StringFieldDefinition<EntityArticle>("id_friendly");
        var indexDefinition = new IndexKeysDefinitionBuilder<EntityArticle>().Ascending(fieldIdFriendly);
        var indexIdFriendly = new CreateIndexModel<EntityArticle>(indexDefinition, new CreateIndexOptions() { Unique = true });
        await collection.Indexes.CreateOneAsync(indexIdFriendly);

        var fieldIdDivision = new StringFieldDefinition<EntityArticle>("Division");
        var indexDivision = new IndexKeysDefinitionBuilder<EntityArticle>().Ascending(fieldIdDivision);
        var indexModelDivision = new CreateIndexModel<EntityArticle>(indexDivision, new CreateIndexOptions() { Unique = false });
        await collection.Indexes.CreateOneAsync(indexModelDivision);
    }
}
