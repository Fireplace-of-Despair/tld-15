using MongoDB.Driver;
using System;

namespace TLD15.Infrastructure;

public static class MongoHelper
{
    public static InsertOneOptions GetDefaultInsert<TDocument>(this IMongoCollection<TDocument> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        return new InsertOneOptions
        {
             BypassDocumentValidation = false,
        };
    }
}
