namespace TTSS.Core.Data;

/// <summary>
/// Contract for all database models.
/// </summary>
public interface IDbModel;

/// <summary>
/// Contract for all database models with a primary key.
/// </summary>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IDbModel<TKey> : IDbModel
{
    /// <summary>
    /// Primary key.
    /// </summary>
    TKey Id { get; set; }
}