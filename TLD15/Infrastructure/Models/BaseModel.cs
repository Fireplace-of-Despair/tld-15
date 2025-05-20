using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models;

/// <summary> Base class for all models in the application. </summary>
public abstract class BaseModel
{
    /// <summary> The date and time when the record was created. </summary>
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary> The date and time when the record was last updated. </summary>
    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary> The version of the record, used for optimistic concurrency control. </summary>
    [Column("version")]
    public long Version { get; set; }

    protected BaseModel()
    {
        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
        Version = 0;
    }
}
