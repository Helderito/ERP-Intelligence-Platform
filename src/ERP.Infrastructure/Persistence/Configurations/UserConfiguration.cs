using ERP.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        builder.HasKey(user => user.Id);

        builder.Ignore(user => user.DomainEvents);

        builder.Property(user => user.Id)
            .ValueGeneratedNever();

        builder.OwnsOne(user => user.Email, emailBuilder =>
        {
            emailBuilder.Property(email => email.Value)
                .HasColumnName("Email")
                .HasMaxLength(320)
                .IsRequired();

            emailBuilder.HasIndex(email => email.Value)
                .IsUnique();
        });

        builder.OwnsOne(user => user.PasswordHash, passwordHashBuilder =>
        {
            passwordHashBuilder.Property(passwordHash => passwordHash.Value)
                .HasColumnName("PasswordHash")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.Property(user => user.CreatedAtUtc)
            .IsRequired();

        builder.Property(user => user.LastAuthenticatedAtUtc);

        builder.Property(user => user.IsActive)
            .IsRequired();

        builder.HasMany<UserRole>("_userRoles")
            .WithOne()
            .HasForeignKey(userRole => userRole.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation("_userRoles")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
