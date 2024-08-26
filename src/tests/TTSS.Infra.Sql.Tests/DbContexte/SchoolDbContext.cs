using Microsoft.EntityFrameworkCore;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql.DbContexte;

internal class SchoolDbContext(DbContextOptions<SchoolDbContext> options) : DbContextBase<SchoolDbContext>(options)
{
    public DbSet<Student> Student { get; set; }
    public DbSet<Teacher> Teacher { get; set; }
    public DbSet<Principal> Principals { get; set; }
}

internal abstract class PersonBase : SqlModelBase
{
    public string Name { get; set; } = string.Empty;
}

internal class Student(Teacher teacher) : PersonBase
{
    public string TeacherId { get; set; } = teacher.Id;
    public Teacher Teacher { get; set; } = teacher ?? throw new ArgumentNullException(nameof(teacher));

    private Student() : this(null!)
    {
    }
}

internal class Teacher : PersonBase
{
    public int Salary { get; set; }
    public ICollection<Student> Students { get; set; } = null!;
}

internal class Principal : IDbModel<int>
{
    public required int Id { get; set; }
    public string Name { get; set; }
}