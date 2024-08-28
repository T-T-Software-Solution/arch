using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using TTSS.Infra.Data.Sql;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TTSS.Core.Data;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Add extension methods for mapping operations to ISqlRepository.
/// </summary>
public static class IRepositoryExtensions
{
    /// <summary>
    /// Configure the repository.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TProperty">The type of the related entity to be included</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="navigationPropertyPath">The related entity to be included</param>
    /// <returns>The repository</returns>
    public static IRepository<TEntity> Include<TEntity, TProperty>(this IRepository<TEntity> target, Expression<Func<TEntity, TProperty>> navigationPropertyPath)
        where TEntity : class, IDbModel<string>
    {
        Include<TEntity, string, TProperty>(target, navigationPropertyPath);
        return target;
    }

    /// <summary>
    /// Configure the repository.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <typeparam name="TProperty">The type of the related entity to be included</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="navigationPropertyPath">The related entity to be included</param>
    /// <returns>The repository</returns>
    public static IRepository<TEntity, TKey> Include<TEntity, TKey, TProperty>(this IRepository<TEntity, TKey> target, Expression<Func<TEntity, TProperty>> navigationPropertyPath)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
    {
        GetConfigurableRepository(target).Configure(it => it.Include(navigationPropertyPath));
        return target;
    }

    /// <summary>
    /// Load a reference property.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    public static IRepository<TEntity> LoadReference<TEntity, TProperty>(this IRepository<TEntity> target, TEntity entity, Expression<Func<TEntity, TProperty?>> propertyExpression)
        where TProperty : class
        where TEntity : class, IDbModel<string>
    {
        LoadReference<TEntity, string, TProperty>(target, entity, propertyExpression);
        return target;
    }

    /// <summary>
    /// Load a reference property.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    public static IRepository<TEntity, TKey> LoadReference<TEntity, TKey, TProperty>(this IRepository<TEntity, TKey> target, TEntity entity, Expression<Func<TEntity, TProperty?>> propertyExpression)
        where TProperty : class
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
    {
        GetSqlRepository(target).LoadReference(entity, propertyExpression);
        return target;
    }

    /// <summary>
    /// Load a reference property.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    /// <returns>Acknowledged</returns>
    public static IRepository<TEntity> LoadReferenceAsync<TEntity, TProperty>(this IRepository<TEntity> target, TEntity entity, Expression<Func<TEntity, TProperty?>> propertyExpression)
        where TProperty : class
        where TEntity : class, IDbModel<string>
    {
        LoadReferenceAsync<TEntity, string, TProperty>(target, entity, propertyExpression);
        return target;
    }

    /// <summary>
    /// Load a reference property.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    /// <returns>Acknowledged</returns>
    public static IRepository<TEntity, TKey> LoadReferenceAsync<TEntity, TKey, TProperty>(this IRepository<TEntity, TKey> target, TEntity entity, Expression<Func<TEntity, TProperty?>> propertyExpression)
        where TProperty : class
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
    {
        GetSqlRepository(target).LoadReferenceAsync(entity, propertyExpression);
        return target;
    }

    /// <summary>
    /// Load a reference collection property.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    public static IRepository<TEntity> LoadReference<TEntity, TProperty>(this IRepository<TEntity> target, TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression)
        where TProperty : class
        where TEntity : class, IDbModel<string>
    {
        LoadReference<TEntity, string, TProperty>(target, entity, propertyExpression);
        return target;
    }

    /// <summary>
    /// Load a reference collection property.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    public static IRepository<TEntity, TKey> LoadReference<TEntity, TKey, TProperty>(this IRepository<TEntity, TKey> target, TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression)
        where TProperty : class
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
    {
        GetSqlRepository(target).LoadReference(entity, propertyExpression);
        return target;
    }

    /// <summary>
    /// Load a reference collection property.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    /// <returns>Acknowledged</returns>
    public static async Task<IRepository<TEntity>> LoadReferenceAsync<TEntity, TProperty>(this IRepository<TEntity> target, TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression)
        where TProperty : class
        where TEntity : class, IDbModel<string>
    {
        await LoadReferenceAsync<TEntity, string, TProperty>(target, entity, propertyExpression);
        return target;
    }

    /// <summary>
    /// Load a reference collection property.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    /// <returns>Acknowledged</returns>
    public static async Task<IRepository<TEntity, TKey>> LoadReferenceAsync<TEntity, TKey, TProperty>(this IRepository<TEntity, TKey> target, TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression)
        where TProperty : class
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
    {
        await GetSqlRepository(target).LoadReferenceAsync(entity, propertyExpression);
        return target;
    }

    /// <summary>
    /// Starts a new transaction.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">The repository</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction.</returns>
    public static IDbContextTransaction BeginTransaction<TEntity>(this IRepository<TEntity> target)
        where TEntity : class, IDbModel<string>
        => BeginTransaction<TEntity, string>(target);

    /// <summary>
    /// Starts a new transaction.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <param name="target">The repository</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction.</returns>
    public static IDbContextTransaction BeginTransaction<TEntity, TKey>(this IRepository<TEntity, TKey> target)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
        => GetSqlRepository(target).BeginTransaction();

    /// <summary>
    /// Starts a new transaction with a given <see cref="IsolationLevel" />.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="isolationLevel">The <see cref="IsolationLevel" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction</returns>
    public static IDbContextTransaction BeginTransaction<TEntity>(this IRepository<TEntity> target, IsolationLevel isolationLevel)
        where TEntity : class, IDbModel<string>
        => BeginTransaction<TEntity, string>(target, isolationLevel);

    /// <summary>
    /// Starts a new transaction with a given <see cref="IsolationLevel" />.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="isolationLevel">The <see cref="IsolationLevel" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction</returns>
    public static IDbContextTransaction BeginTransaction<TEntity, TKey>(this IRepository<TEntity, TKey> target, IsolationLevel isolationLevel)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
        => GetSqlRepository(target).BeginTransaction(isolationLevel);

    /// <summary>
    /// Applies the outstanding operations in the current transaction to the database.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">The repository</param>
    public static void CommitTransaction<TEntity>(this IRepository<TEntity> target)
        where TEntity : class, IDbModel<string>
        => CommitTransaction<TEntity, string>(target);

    /// <summary>
    /// Applies the outstanding operations in the current transaction to the database.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <param name="target">The repository</param>
    public static void CommitTransaction<TEntity, TKey>(this IRepository<TEntity, TKey> target)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
        => GetSqlRepository(target).CommitTransaction();

    /// <summary>
    /// Discards the outstanding operations in the current transaction.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public static void RollbackTransaction<TEntity>(this IRepository<TEntity> target)
        where TEntity : class, IDbModel<string>
        => RollbackTransaction<TEntity, string>(target);

    /// <summary>
    /// Discards the outstanding operations in the current transaction.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    public static void RollbackTransaction<TEntity, TKey>(this IRepository<TEntity, TKey> target)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
        => GetSqlRepository(target).RollbackTransaction();

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    public static IDbContextTransaction? UseTransaction<TEntity>(this IRepository<TEntity> target, DbTransaction? transaction)
        where TEntity : class, IDbModel<string>
        => UseTransaction<TEntity, string>(target, transaction);

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    public static IDbContextTransaction? UseTransaction<TEntity, TKey>(this IRepository<TEntity, TKey> target, DbTransaction? transaction)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
        => GetSqlRepository(target).UseTransaction(transaction);

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <param name="transactionId">The unique identifier for the transaction.</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    public static IDbContextTransaction? UseTransaction<TEntity>(this IRepository<TEntity> target, DbTransaction? transaction, Guid transactionId)
        where TEntity : class, IDbModel<string>
        => UseTransaction<TEntity, string>(target, transaction, transactionId);

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <param name="transactionId">The unique identifier for the transaction.</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    public static IDbContextTransaction? UseTransaction<TEntity, TKey>(this IRepository<TEntity, TKey> target, DbTransaction? transaction, Guid transactionId)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
        => GetSqlRepository(target).UseTransaction(transaction, transactionId);

    /// <summary>
    /// Asynchronously starts a new transaction.
    /// </summary>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction.</returns>
    public static Task<IDbContextTransaction> BeginTransactionAsync<TEntity>(this IRepository<TEntity> target)
        where TEntity : class, IDbModel<string>
        => BeginTransactionAsync<TEntity, string>(target);

    /// <summary>
    /// Asynchronously starts a new transaction.
    /// </summary>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction.</returns>
    public static Task<IDbContextTransaction> BeginTransactionAsync<TEntity, TKey>(this IRepository<TEntity, TKey> target)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
        => GetSqlRepository(target).BeginTransactionAsync();

    /// <summary>
    /// Asynchronously starts a new transaction with a given <see cref="IsolationLevel" />.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="isolationLevel">The <see cref="IsolationLevel" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction</returns>
    public static Task<IDbContextTransaction> BeginTransactionAsync<TEntity>(this IRepository<TEntity> target, IsolationLevel isolationLevel)
        where TEntity : class, IDbModel<string>
        => BeginTransactionAsync<TEntity, string>(target, isolationLevel);

    /// <summary>
    /// Asynchronously starts a new transaction with a given <see cref="IsolationLevel" />.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="isolationLevel">The <see cref="IsolationLevel" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction</returns>
    public static Task<IDbContextTransaction> BeginTransactionAsync<TEntity, TKey>(this IRepository<TEntity, TKey> target, IsolationLevel isolationLevel)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
        => GetSqlRepository(target).BeginTransactionAsync(isolationLevel);

    /// <summary>
    /// Applies the outstanding operations in the current transaction to the database.
    /// </summary>
    public static Task CommitTransactionAsync<TEntity>(this IRepository<TEntity> target)
        where TEntity : class, IDbModel<string>
        => CommitTransactionAsync<TEntity, string>(target);

    /// <summary>
    /// Applies the outstanding operations in the current transaction to the database.
    /// </summary>
    public static Task CommitTransactionAsync<TEntity, TKey>(this IRepository<TEntity, TKey> target)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
        => GetSqlRepository(target).CommitTransactionAsync();

    /// <summary>
    /// Discards the outstanding operations in the current transaction.
    /// </summary>
    public static Task RollbackTransactionAsync<TEntity>(this IRepository<TEntity> target)
        where TEntity : class, IDbModel<string>
        => RollbackTransactionAsync<TEntity, string>(target);

    /// <summary>
    /// Discards the outstanding operations in the current transaction.
    /// </summary>
    public static Task RollbackTransactionAsync<TEntity, TKey>(this IRepository<TEntity, TKey> target)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
        => GetSqlRepository(target).RollbackTransactionAsync();


    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    public static Task<IDbContextTransaction?> UseTransactionAsync<TEntity>(this IRepository<TEntity> target, DbTransaction? transaction)
        where TEntity : class, IDbModel<string>
        => UseTransactionAsync<TEntity, string>(target, transaction);

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    public static Task<IDbContextTransaction?> UseTransactionAsync<TEntity, TKey>(this IRepository<TEntity, TKey> target, DbTransaction? transaction)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
        => GetSqlRepository(target).UseTransactionAsync(transaction);

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <param name="transactionId">The unique identifier for the transaction.</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    public static Task<IDbContextTransaction?> UseTransactionAsync<TEntity>(this IRepository<TEntity> target, DbTransaction? transaction, Guid transactionId)
        where TEntity : class, IDbModel<string>
        => UseTransactionAsync<TEntity, string>(target, transaction, transactionId);

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <param name="transactionId">The unique identifier for the transaction.</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    public static Task<IDbContextTransaction?> UseTransactionAsync<TEntity, TKey>(this IRepository<TEntity, TKey> target, DbTransaction? transaction, Guid transactionId)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
        => GetSqlRepository(target).UseTransactionAsync(transaction, transactionId);

    private static ISqlRepository<TEntity, TKey> GetSqlRepository<TEntity, TKey>(IRepository<TEntity, TKey> target)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
    {
        if (target is not ISqlRepository<TEntity, TKey> repository)
        {
            throw new NotSupportedException("The repository does not support this operation.");
        }
        return repository;
    }

    private static IConfigurableRepository<TEntity> GetConfigurableRepository<TEntity, TKey>(IRepository<TEntity, TKey> target)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
    {
        if (target is not IConfigurableRepository<TEntity> repository)
        {
            throw new NotSupportedException("The repository does not support this operation.");
        }
        return repository;
    }
}