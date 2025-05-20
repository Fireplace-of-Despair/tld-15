using Infrastructure.Composition;
using Infrastructure.Models.Reference;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models.Business;

[Table("article", Schema = SchemaName.Business)]
public sealed class Article : BaseModel
{
    [Key, Column("id")]
    public required string Id { get; set; }

    [Column("division_id")]
    public string DivisionId { get; set; } = string.Empty;

    [Column("poster_url")]
    public string PosterUrl { get; set; } = string.Empty;

    public Division Division { get; set; } = null!;

    public ICollection<ArticleTranslation> Translations { get; set; } = [];
}

[Table("article_translation", Schema = SchemaName.Business)]
public sealed class ArticleTranslation : BaseModel
{
    [Key, Column("id")]
    public required Guid Id { get; set; }

    [Column("article_id")]
    public required string ArticleId { get; set; }

    [Column("language_id")]
    public required string LanguageId { get; set; }

    [Column("poster_alt")]
    public string PosterAlt { get; set; } = string.Empty;

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("subtitle")]
    public string Subtitle { get; set; } = string.Empty;

    [Column("content_html")]
    public string ContentHtml { get; set; } = string.Empty;
}