using ERP.Domain.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations.MasterData;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");

        builder.HasKey(product => product.Id);

        builder.Ignore(product => product.DomainEvents);

        builder.Property(product => product.Id)
            .ValueGeneratedNever();

        builder.OwnsOne(product => product.Code, codeBuilder =>
        {
            codeBuilder.Property(code => code.Value)
                .HasColumnName("Code")
                .HasMaxLength(50)
                .IsRequired();

            codeBuilder.HasIndex(code => code.Value)
                .IsUnique();
        });

        builder.Property(product => product.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.CategoryId)
            .IsRequired();

        builder.Property(product => product.UnitOfMeasureId)
            .IsRequired();

        builder.Property(product => product.IsActive)
            .IsRequired();

        builder.Property(product => product.CreatedAtUtc)
            .IsRequired();

        builder.Property(product => product.UpdatedAtUtc);

        builder.Property(product => product.DeactivatedAtUtc);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UnitOfMeasure>()
            .WithMany()
            .HasForeignKey(product => product.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
