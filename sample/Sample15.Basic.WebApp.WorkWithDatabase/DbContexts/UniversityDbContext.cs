using Microsoft.EntityFrameworkCore;
using Sample15.Basic.WebApp.WorkWithDatabase.Entities;
using TTSS.Infra.Data.Sql;

namespace Sample15.Basic.WebApp.WorkWithDatabase.DbContexts;

public sealed class UniversityDbContext(DbContextOptions<UniversityDbContext> options) : DbContextBase<UniversityDbContext>(options)
{
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Student> Students { get; set; }
}