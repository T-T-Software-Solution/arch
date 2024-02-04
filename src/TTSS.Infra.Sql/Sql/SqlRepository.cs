using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using TTSS.Core.Data;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// SQL implementation of <see cref="IRepository{TEntity, TKey}"/>.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public class SqlRepository<TEntity, TKey> : ISqlRepository<TEntity, TKey>
    where TEntity : class, IDbModel<TKey>
    where TKey : notnull
{
    #region Fields

    private DbSet<TEntity> _collection;
    private List<string> _includePropertyPaths = new();

    #endregion

    #region Properties

    /// <summary>
    /// Entity framework core DbContext.
    /// </summary>
    protected internal DbContext DbContext { get; }

    /// <summary>
    /// Entity framework core DbSet.
    /// </summary>
    protected internal DbSet<TEntity> Collection
    {
        get
        {
            IQueryable<TEntity> qry = _collection;
            foreach (var path in _includePropertyPaths)
            {
                qry = qry.Include(path);
            }
            return (DbSet<TEntity>)qry;
        }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlRepository{TEntity, TKey}"/> class.
    /// </summary>
    /// <param name="connectionStore">The connection store</param>
    /// <param name="dbContextFactory">The DbContext factory</param>
    /// <exception cref="ArgumentOutOfRangeException">All parameters are required</exception>
    public SqlRepository(SqlConnectionStore connectionStore, SqlDbContextFactory dbContextFactory)
    {
        var collection = connectionStore.GetCollection<TEntity>(dbContextFactory);
        DbContext = collection.dbContext ?? throw new ArgumentOutOfRangeException(nameof(collection.dbContext));
        _collection = collection.collection ?? throw new ArgumentOutOfRangeException(nameof(collection.collection));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get an entity by id.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entity</returns>
    public async Task<TEntity?> GetByIdAsync(TKey key, CancellationToken cancellationToken = default)
        => await Collection.FindAsync(new object?[] { key }, cancellationToken: cancellationToken);

    /// <summary>
    /// Get all data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entities</returns>
    public IEnumerable<TEntity> Get(CancellationToken cancellationToken = default)
        => new SqlQueryResult<TEntity>(Collection, cancellationToken);

    /// <summary>
    /// Get data by filter.
    /// </summary>
    /// <param name="filter">Entity filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entities</returns>
    public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        => new SqlQueryResult<TEntity>(Collection.Where(filter), cancellationToken);

    /// <summary>
    /// Convert to queryable.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Queryable</returns>
    public IQueryable<TEntity> Query(CancellationToken cancellationToken = default)
        => Collection;

    /// <summary>
    /// Insert an entity.
    /// </summary>
    /// <param name="entity">Entity to insert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        => InsertRecordAsync(entity, cancellationToken);

    /// <summary>
    /// Update an entity.
    /// </summary>
    /// <param name="entity">Entity to insert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        => UpdateAsync(entity.Id, entity, cancellationToken);

    /// <summary>
    /// Update an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="entity">Entity to insert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> UpdateAsync(TKey key, TEntity entity, CancellationToken cancellationToken = default)
    {
        if (!key?.Equals(entity.Id) ?? false) return Task.FromResult(false);
        Collection.Update(entity);
        return SaveChangedAsync(cancellationToken);
    }

    /// <summary>
    /// Upsert an entity.
    /// </summary>
    /// <param name="entity">Entity to upsert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> UpsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        => UpsertAsync(entity.Id, entity, cancellationToken);

    /// <summary>
    /// Update an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="entity">Entity to insert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public async Task<bool> UpsertAsync(TKey key, TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null) return false;
        var canUpdate = null != await GetByIdAsync(key, cancellationToken);
        return canUpdate
            ? await UpdateAsync(entity, cancellationToken)
            : await InsertRecordAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Delete an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public async Task<bool> DeleteAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var target = await GetByIdAsync(key, cancellationToken);
        if (null == target) return false;
        Collection.Remove(target);
        return await SaveChangedAsync(cancellationToken);
    }

    /// <summary>
    /// Delete entities.
    /// </summary>
    /// <param name="filter">Entity filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> DeleteManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        var targets = Query(cancellationToken).Where(filter);
        if (false == (targets?.Any() ?? false)) return Task.FromResult(false);
        Collection.RemoveRange(targets);
        return SaveChangedAsync(cancellationToken);
    }

    /// <summary>
    /// Insert entities in bulk operation.
    /// </summary>
    /// <param name="entities">Entities to insert bulk</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public async Task InsertBulkAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        if (false == (entities?.Any() ?? false)) return;
        // HACK: Temporary use. (The Library will be determined later)
        await Collection.AddRangeAsync(entities, cancellationToken);
        await SaveChangedAsync(cancellationToken);
    }

    #region Transactions

    /// <summary>
    /// Starts a new transaction.
    /// </summary>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction.</returns>
    public IDbContextTransaction BeginTransaction()
        => DbContext.Database.BeginTransaction();

    /// <summary>
    /// Starts a new transaction with a given <see cref="IsolationLevel" />.
    /// </summary>
    /// <param name="isolationLevel">The <see cref="IsolationLevel" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction</returns>
    public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        => DbContext.Database.BeginTransaction(isolationLevel);

    /// <summary>
    /// Applies the outstanding operations in the current transaction to the database.
    /// </summary>
    public void CommitTransaction()
        => DbContext.Database.CommitTransaction();

    /// <summary>
    /// Discards the outstanding operations in the current transaction.
    /// </summary>
    public void RollbackTransaction()
        => DbContext.Database.RollbackTransaction();

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    public IDbContextTransaction? UseTransaction(DbTransaction? transaction)
        => DbContext.Database.UseTransaction(transaction);

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <param name="transactionId">The unique identifier for the transaction.</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    public IDbContextTransaction? UseTransaction(DbTransaction? transaction, Guid transactionId)
        => DbContext.Database.UseTransaction(transaction, transactionId);

    /// <summary>
    /// Asynchronously starts a new transaction.
    /// </summary>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction.</returns>
    public Task<IDbContextTransaction> BeginTransactionAsync()
        => DbContext.Database.BeginTransactionAsync();

    /// <summary>
    /// Asynchronously starts a new transaction with a given <see cref="IsolationLevel" />.
    /// </summary>
    /// <param name="isolationLevel">The <see cref="IsolationLevel" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that represents the started transaction</returns>
    public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel)
        => DbContext.Database.BeginTransactionAsync(isolationLevel);

    /// <summary>
    /// Applies the outstanding operations in the current transaction to the database.
    /// </summary>
    public Task CommitTransactionAsync()
        => DbContext.Database.CommitTransactionAsync();

    /// <summary>
    /// Discards the outstanding operations in the current transaction.
    /// </summary>
    public Task RollbackTransactionAsync()
        => DbContext.Database.RollbackTransactionAsync();

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    public Task<IDbContextTransaction?> UseTransactionAsync(DbTransaction? transaction)
        => DbContext.Database.UseTransactionAsync(transaction);

    /// <summary>
    /// Sets the <see cref="DbTransaction" /> to be used by database operations on the <see cref="DbContext" />.
    /// </summary>
    /// <param name="transaction">The <see cref="DbTransaction" /> to use</param>
    /// <param name="transactionId">The unique identifier for the transaction.</param>
    /// <returns>A <see cref="IDbContextTransaction" /> that encapsulates the given transaction</returns>
    public Task<IDbContextTransaction?> UseTransactionAsync(DbTransaction? transaction, Guid transactionId)
        => DbContext.Database.UseTransactionAsync(transaction, transactionId);

    #endregion

    #region ISqlRepositorySpecific members

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
    public ISqlRepositorySpecific<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigationPropertyPath) where TProperty : class
    {
        if (navigationPropertyPath.Body is MemberExpression expression && false == _includePropertyPaths.Contains(expression.Member.Name))
        {
            _includePropertyPaths.Add(expression.Member.Name);
        }
        return this;
    }

    /// <summary>
    /// Load a reference property.
    /// </summary>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    public void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> propertyExpression) where TProperty : class
        => Collection.Entry(entity).Reference(propertyExpression).Load();

    /// <summary>
    /// Load a reference property.
    /// </summary>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    /// <returns>Acknowledged</returns>
    public Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> propertyExpression) where TProperty : class
        => Collection.Entry(entity).Reference(propertyExpression).LoadAsync();

    /// <summary>
    /// Load a reference collection property.
    /// </summary>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    public void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression) where TProperty : class
        => Collection.Entry(entity).Collection(propertyExpression).Load();

    /// <summary>
    /// Load a reference collection property.
    /// </summary>
    /// <typeparam name="TProperty">The reference property type</typeparam>
    /// <param name="entity">The entity</param>
    /// <param name="propertyExpression">Reference property selector</param>
    /// <returns>Acknowledged</returns>
    public Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression) where TProperty : class
        => Collection.Entry(entity).Collection(propertyExpression).LoadAsync();

    #endregion

    #region IAsyncQueryRepository members

    /// <summary>
    /// Get entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entities</returns>
    public IAsyncEnumerable<TEntity> GetAsync(CancellationToken cancellationToken = default)
        => Collection.AsAsyncEnumerable();

    /// <summary>
    /// Get entities.
    /// </summary>
    /// <param name="filter">Entity filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entities</returns>
    public IAsyncEnumerable<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        => Collection.Where(filter).AsAsyncEnumerable();

    #endregion

    #region IDisposable members

    /// <summary>
    /// Release resources.
    /// </summary>
    public void Dispose()
    {
        DbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion

    #region IAsyncDisposable members

    /// <summary>
    /// Release resources.
    /// </summary>
    /// <returns>Acknowledgement</returns>
    public ValueTask DisposeAsync()
    {
        var result = DbContext.DisposeAsync();
        GC.SuppressFinalize(this);
        return result;
    }

    #endregion

    private async Task<bool> SaveChangedAsync(CancellationToken cancellationToken)
        => await DbContext.SaveChangesAsync(cancellationToken) > 0;

    private async Task<bool> InsertRecordAsync(TEntity data, CancellationToken cancellationToken = default)
    {
        await Collection.AddAsync(data, cancellationToken);
        return await SaveChangedAsync(cancellationToken);
    }

    #endregion
}

/// <summary>
/// SQL implementation of <see cref="IRepository{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public class SqlRepository<TEntity> : SqlRepository<TEntity, string>,
    ISqlRepository<TEntity>
    where TEntity : class, IDbModel<string>
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="connectionStore">The connection store</param>
    /// <param name="dbContextFactory">The DbContext factory</param>
    public SqlRepository(SqlConnectionStore connectionStore, SqlDbContextFactory dbContextFactory)
        : base(connectionStore, dbContextFactory)
    {
    }

    #endregion
}