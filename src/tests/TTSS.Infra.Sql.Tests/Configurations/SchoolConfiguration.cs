using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TTSS.Infra.Data.Sql.DbContexte;

namespace TTSS.Infra.Data.Sql.Configurations;

internal class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder
            .Property(it => it.Name)
            .HasMaxLength(100)
            .IsRequired(true);

        builder
            .HasOne(it => it.Teacher)
            .WithMany(it => it.Students)
            .HasForeignKey(it => it.TeacherId);
    }
}

internal class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder
            .Property(it => it.Name)
            .HasMaxLength(100)
            .IsRequired(true);

        builder
            .HasMany(it => it.Students)
            .WithOne(it => it.Teacher)
            .HasForeignKey(it => it.TeacherId);
    }
}

internal class PrincipalConfiguration : IEntityTypeConfiguration<Principal>
{
    public void Configure(EntityTypeBuilder<Principal> builder)
    {
        builder
            .Property(it => it.Name)
            .HasMaxLength(100)
            .IsRequired(true);
    }
}