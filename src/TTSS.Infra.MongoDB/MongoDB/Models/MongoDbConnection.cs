namespace TTSS.Infra.Data.MongoDB.Models;

/// <summary>
/// MongoDB connection information.
/// </summary>
public sealed record MongoDbConnection
{
    #region Properties

    /// <summary>
    /// Data type name.
    /// </summary>
    public required string TypeName { get; init; }

    /// <summary>
    /// Target collection name.
    /// </summary>
    public required string CollectionName { get; init; }

    /// <summary>
    /// Target database name.
    /// </summary>
    public required string DatabaseName { get; init; }

    /// <summary>
    /// Connection string.
    /// </summary>
    public required string ConnectionString { get; init; }

    /// <summary>
    /// Configures the collection to not use a discriminator.
    /// </summary>
    public required bool NoDiscriminator { get; init; }

    /// <summary>
    /// Indicates if the collection is a child of another collection.
    /// </summary>
    public required bool IsChild { get; init; }

    #endregion
}