using MongoDB.Bson.Serialization.Attributes;
using TTSS.Core.Data;

namespace TTSS.Infra.Data.MongoDB.Models;

/// <summary>
/// Base class for MongoDB entities.
/// </summary>
public abstract class MongoDbDocumentBase : IDbModel<string>
{
    #region Properties

    /// <summary>
    /// Primary key of the entity.
    /// </summary>
    [BsonId]
    public required string Id { get; set; }

    #endregion
}