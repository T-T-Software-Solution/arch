using Microsoft.EntityFrameworkCore;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql.DbContexte;

internal class SchoolDbContext : DbContextBase<SchoolDbContext>
{
    public DbSet<Student> Student { get; set; }
    public DbSet<Teacher> Teacher { get; set; }
    public DbSet<Principal> Principals { get; set; }

    public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options)
    {
    }
}

internal abstract class PersonBase : SqlModelBase
{
    public string Name { get; set; } = string.Empty;
}

internal class Student : PersonBase
{
    public string TeacherId { get; set; }
    public Teacher Teacher { get; set; }

    public Student(Teacher teacher)
    {
        Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));
        TeacherId = teacher.Id;
    }

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