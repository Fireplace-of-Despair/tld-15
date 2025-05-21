using Infrastructure.Composition;
using Infrastructure.Models.Reference;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Infrastructure.Models.Business;

[Table("project", Schema = SchemaName.Business)]
public sealed class Project : BaseModel
{
    [Key, Column("id")]
    public required string Id { get; set; }

    [Column("division_id")]
    public string DivisionId { get; set; } = string.Empty;

    [Column("poster_url")]
    public string PosterUrl { get; set; } = string.Empty;

    [Column("links")]
    public string Links { get; set; } = string.Empty;

    [ForeignKey(nameof(DivisionId))]
    public Division Division { get; set; } = null!;

    public ICollection<ProjectTranslation> Translations { get; set; } = [];


    public static Dictionary<string, string> LinksToDictionary(string links)
    {
        return JsonSerializer.Deserialize<Dictionary<string, string>>(links) ?? [];
    }

    [NotMapped]
    public static string LinksDefault
    {
        get
        {
            return JsonSerializer.Serialize(new Dictionary<string, string>()
                {
                    { "github_", "%url%" },
                    { "amazon_en", "%url%" },
                });
        }
    }
}

[Table("project_translation", Schema = SchemaName.Business)]
public sealed class ProjectTranslation : BaseModel
{
    [Key, Column("id")]
    public required Guid Id { get; set; }

    [Column("project_id")]
    public required string ProjectId { get; set; }

    [Column("language_id")]
    public string LanguageId { get; set; } = string.Empty;

    [Column("poster_alt")]
    public string PosterAlt { get; set; } = string.Empty;

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("subtitle")]
    public string Subtitle { get; set; } = string.Empty;

    [Column("content_html")]
    public string ContentHtml { get; set; } = string.Empty;
}