using MongoDB.Driver;
using System.Threading.Tasks;

namespace Infrastructure.Entities;

public interface IEntityStored
{
    public static abstract string Collection { get; }

    public static abstract Task CreateIndexesAsync(IMongoDatabase database);
}
