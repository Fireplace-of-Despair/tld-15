using MongoDB.Driver;
using System.Threading.Tasks;

namespace Infrastructure.Entities;

public interface IEntityStored
{
    public static abstract string Collection { get; }
    public static abstract string Database { get; }

    public static abstract Task CreateIndexesAsync(IMongoClient client);
}
