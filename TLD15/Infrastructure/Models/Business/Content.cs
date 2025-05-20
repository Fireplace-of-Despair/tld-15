using Infrastructure.Composition;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models.Business;

[Table("content", Schema = SchemaName.Business)]
public sealed class Content : BaseModel
{
    [Key, Column("id")]
    public required string Id { get; set; }

    public ICollection<ContentTranslation> Translations { get; set; } = [];
}

[Table("content_translation", Schema = SchemaName.Business)]
public sealed class ContentTranslation : BaseModel
{
    [Key, Column("id")]
    public required Guid Id { get; set; }

    [Column("content_id")]
    public required string ContentId { get; set; }

    [Column("language_id")]
    public required string LanguageId { get; set; }

    [Column("data")]
    public string Data { get; set; } = string.Empty;
}