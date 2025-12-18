using LendingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LendingService.Persistence.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(l => new { l.BookId, l.IsActive });
        builder.HasIndex(l => l.UserId);

        builder.Property(l => l.IsActive)
            .HasDefaultValue(true);
    }
}