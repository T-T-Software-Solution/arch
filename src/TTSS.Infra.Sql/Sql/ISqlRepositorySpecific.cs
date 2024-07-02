using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using TTSS.Core.Data;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// Contract for SQL repository specific.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface ISqlRepositorySpecific<TEntity> : IRepositoryBase
{
    /// <summary>
    /// Specifies related entities to include in the query results. The navigation property to be included is specified starting with the type of entity being queried (<typeparamref name="TEntity" />).
    /// </summary>
    /// <remarks>
    /// See <see href="https://aka.ms/efcore-docs-load-related-data">Loading related entities</see> for more information and examples.
    /// </remarks>
    /// <typeparam name="TProperty">The type of the related entity to be included</typeparam>
    /// <param name="navigationPropertyPath">
    /// A lambda expression representing the navigation property to be included (<c>t => t.Property1</c>).
    /// </param>
    ISqlRepositorySpecific<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty?>> navigationPropertyPath) where TProperty : class;

    /// <summary>
    /// Load a reference property.
    /// </summary>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> propertyExpression) where TProperty : class;

    /// <summary>
    /// Load a reference property.
    /// </summary>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    /// <returns>Acknowledged</returns>
    Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> propertyExpression) where TProperty : class;

    /// <summary>
    /// Load a reference collection property.
    /// </summary>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression) where TProperty : class;

    /// <summary>
    /// Load a reference collection property.
    /// </summary>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    /// <returns>Acknowledged</returns>
    Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression) where TProperty : class;

    /// <summary>
    /// Starts a new transaction.
    /// </summary>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction.</returns>
    IDbContextTransaction BeginTransaction();

    /// <summary>
    /// Starts a new transaction with a given <see cref="IsolationLevel" />.
    /// </summary>
    /// <param name="isolationLevel">The <see cref="IsolationLevel" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction</returns>
    IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);

    /// <summary>
    /// Applies the outstanding operations in the current transaction to the database.
    /// </summary>
    void CommitTransaction();

    /// <summary>
    /// Discards the outstanding operations in the current transaction.
    /// </summary>
    void RollbackTransaction();

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    IDbContextTransaction? UseTransaction(DbTransaction? transaction);

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <param name="transactionId">The unique identifier for the transaction.</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    IDbContextTransaction? UseTransaction(DbTransaction? transaction, Guid transactionId);

    /// <summary>
    /// Asynchronously starts a new transaction.
    /// </summary>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction.</returns>
    Task<IDbContextTransaction> BeginTransactionAsync();

    /// <summary>
    /// Asynchronously starts a new transaction with a given <see cref="IsolationLevel" />.
    /// </summary>
    /// <param name="isolationLevel">The <see cref="IsolationLevel" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel);

    /// <summary>
    /// Applies the outstanding operations in the current transaction to the database.
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// Discards the outstanding operations in the current transaction.
    /// </summary>
    Task RollbackTransactionAsync();

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    Task<IDbContextTransaction?> UseTransactionAsync(DbTransaction? transaction);

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <param name="transactionId">The unique identifier for the transaction.</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    Task<IDbContextTransaction?> UseTransactionAsync(DbTransaction? transaction, Guid transactionId);

    /// <summary>
    /// Filters the elements of an System.Linq.IQueryable based on a specified type.
    /// </summary>
    /// <typeparam name="TResult">The type to filter the elements of the sequence on</typeparam>
    /// <returns>A collection that contains the elements from source that have type TResult</returns>
    IQueryable<TResult> OfType<TResult>();
}