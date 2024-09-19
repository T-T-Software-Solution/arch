using Microsoft.EntityFrameworkCore;
using TTSS.Infra.Data.Sql;

namespace Sample18.RemotePublish.Shared.DbContexts;

public sealed class TransportDbContext(DbContextOptions<TransportDbContext> options) : DbContextBase(options);