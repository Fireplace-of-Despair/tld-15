using Infrastructure.Composition;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models.Reference;
[Table("language", Schema = SchemaName.Reference)]
public sealed class Language : BaseModel
{
    [Key, Column("id")]
    public required string Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }
}