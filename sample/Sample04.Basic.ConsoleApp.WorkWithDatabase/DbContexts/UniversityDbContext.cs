using Microsoft.EntityFrameworkCore;
using Sample04.Basic.ConsoleApp.WorkWithDatabase.Entities;
using TTSS.Infra.Data.Sql;

namespace Sample04.Basic.ConsoleApp.WorkWithDatabase.DbContexts;

public sealed class UniversityDbContext(DbContextOptions<UniversityDbContext> options) : DbContextBase<UniversityDbContext>(options)
{
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Student> Students { get; set; }
}