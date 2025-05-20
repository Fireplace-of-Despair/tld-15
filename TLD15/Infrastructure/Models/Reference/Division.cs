using Infrastructure.Composition;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models.Reference;
[Table("division", Schema = SchemaName.Reference)]
public sealed class Division : BaseModel
{
    [Key, Column("id")]
    public required string Id { get; set; }

    public ICollection<DivisionTranslation> Translations { get; set; } = [];
}

[Table("division_translation", Schema = SchemaName.Reference)]
public sealed class DivisionTranslation : BaseModel
{
    [Key, Column("id")]
    public required Guid Id { get; set; }

    [Column("division_id")]
    public required string DivisionId { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("language_id")]
    public required string LanguageId { get; set; }
}