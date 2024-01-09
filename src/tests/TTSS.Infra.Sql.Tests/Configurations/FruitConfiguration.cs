using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TTSS.Infra.Data.Sql.DbContexte;

namespace TTSS.Infra.Data.Sql.Configurations;

internal class AppleConfiguration : IEntityTypeConfiguration<Apple>
{
    public void Configure(EntityTypeBuilder<Apple> builder)
    {
        builder
            .Property(it => it.Name)
            .HasMaxLength(100)
            .IsRequired(true);
    }
}

internal class BananaConfiguration : IEntityTypeConfiguration<Banana>
{
    public void Configure(EntityTypeBuilder<Banana> builder)
    {
        builder
            .Property(it => it.Name)
            .HasMaxLength(100)
            .IsRequired(true);
    }
}