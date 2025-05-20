using Infrastructure.Composition;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models.Identity;
[Table("account", Schema = SchemaName.Identity)]
public sealed class Account : BaseModel
{
    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; set; }

    [Column("login")]
    public string Login { get; set; } = string.Empty;

    [Column("password_hash")]
    public string Password { get; set; } = string.Empty;

    [Column("password_salt")]
    public string Salt { get; set; } = string.Empty;
}
