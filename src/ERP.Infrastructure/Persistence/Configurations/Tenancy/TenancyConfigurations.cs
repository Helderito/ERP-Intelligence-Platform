using ERP.Domain.Identity;
using ERP.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations.Tenancy;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Company");
        builder.HasKey(company => company.Id);
        builder.Ignore(company => company.DomainEvents);
        builder.Property(company => company.Id).ValueGeneratedNever();
        builder.Property(company => company.Name).HasMaxLength(200).IsRequired();
        builder.Property(company => company.IsActive).IsRequired();
        builder.Property(company => company.CreatedAtUtc).IsRequired();
        builder.Property(company => company.UpdatedAtUtc);
        builder.Property(company => company.DeactivatedAtUtc);

        builder.HasMany(company => company.Establishments)
            .WithOne()
            .HasForeignKey(establishment => establishment.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(company => company.Establishments).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsOne(company => company.FiscalProfile, profileBuilder =>
        {
            profileBuilder.ToTable("CompanyFiscalProfile");
            profileBuilder.Property(profile => profile.VatRegime).HasConversion<string>().HasMaxLength(20).IsRequired();
            profileBuilder.OwnsOne(profile => profile.Nif, nifBuilder =>
            {
                nifBuilder.Property(nif => nif.Value).HasColumnName("Nif").HasMaxLength(20).IsRequired();
            });
            profileBuilder.OwnsOne(profile => profile.FiscalAddress, addressBuilder =>
            {
                addressBuilder.Property(address => address.Value)
                    .HasColumnName("FiscalAddress")
                    .HasMaxLength(500)
                    .IsRequired();
            });
        });

        builder.HasData(new
        {
            Id = TenancySeed.DefaultCompanyId,
            Name = "Empresa Principal",
            IsActive = true,
            CreatedAtUtc = TenancySeed.SeededAtUtc,
            UpdatedAtUtc = (DateTime?)null,
            DeactivatedAtUtc = (DateTime?)null
        });
    }
}

public sealed class EstablishmentConfiguration : IEntityTypeConfiguration<Establishment>
{
    public void Configure(EntityTypeBuilder<Establishment> builder)
    {
        builder.ToTable("Establishment");
        builder.HasKey(establishment => establishment.Id);
        builder.Ignore(establishment => establishment.DomainEvents);
        builder.Property(establishment => establishment.Id).ValueGeneratedNever();
        builder.Property(establishment => establishment.CompanyId).IsRequired();
        builder.Property(establishment => establishment.Code).HasMaxLength(50).IsRequired();
        builder.Property(establishment => establishment.Name).HasMaxLength(200).IsRequired();
        builder.Property(establishment => establishment.EstablishmentNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(establishment => new { establishment.CompanyId, establishment.Code }).IsUnique();
        builder.HasData(new
        {
            Id = TenancySeed.DefaultEstablishmentId,
            CompanyId = TenancySeed.DefaultCompanyId,
            Code = "MAIN",
            Name = "Sede",
            EstablishmentNumber = "001"
        });
    }
}

public sealed class UserCompanyConfiguration : IEntityTypeConfiguration<UserCompany>
{
    public void Configure(EntityTypeBuilder<UserCompany> builder)
    {
        builder.ToTable("UserCompany");
        builder.HasKey(membership => membership.Id);
        builder.Ignore(membership => membership.DomainEvents);
        builder.Property(membership => membership.Id).ValueGeneratedNever();
        builder.Property(membership => membership.UserId).IsRequired();
        builder.Property(membership => membership.CompanyId).IsRequired();
        builder.Property(membership => membership.AssignedAtUtc).IsRequired();
        builder.HasIndex(membership => membership.UserId).IsUnique();
        builder.HasIndex(membership => membership.CompanyId);
        builder.HasOne<User>().WithMany().HasForeignKey(membership => membership.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Company>().WithMany().HasForeignKey(membership => membership.CompanyId).OnDelete(DeleteBehavior.Restrict);
    }
}
