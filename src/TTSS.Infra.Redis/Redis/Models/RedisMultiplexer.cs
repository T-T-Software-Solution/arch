using StackExchange.Redis;

namespace TTSS.Infra.Data.Redis.Models;

internal sealed class RedisMultiplexer(RedisConnection connection) : IDisposable
{
    #region Fields

    private IDatabase? _database;
    private volatile ConnectionMultiplexer _connectionMultiplexer = null!;
    private readonly SemaphoreSlim _theLock = new(initialCount: 1, maxCount: 1);

    #endregion

    #region Methods

    public async ValueTask<IDatabase> GetDatabaseAsync(CancellationToken token = default)
        => _database ?? await ConnectAsync(token);

    public void Dispose()
        => _connectionMultiplexer?.Close();

    private async Task<IDatabase> ConnectAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (_database != null) return _database;

        var connectionLock = _theLock;
        await connectionLock.WaitAsync(token);
        try
        {
            if (_database == null)
            {
                _connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connection.ConnectionString);
                _database = _connectionMultiplexer.GetDatabase();
            }
        }
        finally
        {
            connectionLock.Release();
        }

        return _database;
    }

    #endregion
}