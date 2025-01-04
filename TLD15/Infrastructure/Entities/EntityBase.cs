using ACherryPie.Incidents;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Infrastructure.Entities;

public abstract class EntityBase<T>
{
    protected EntityBase()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Version = 0;
    }

    [BsonId, BsonElement("_id")]
    public required T Id { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("version")]
    public long Version { get; set; }

    public virtual void Bump(long version)
    {
        UpdatedAt = DateTime.UtcNow;

        if(version != Version)
        {
            throw new IncidentException(IncidentCode.VersionMismatch);
        }
        Version++;
    }
}
