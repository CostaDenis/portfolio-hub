using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Infrastructure.Data.Mappings;

public class DividendMapping : IEntityTypeConfiguration<Dividend>
{
    public void Configure(EntityTypeBuilder<Dividend> builder)
    {
        builder.ToTable("Dividends");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasColumnType("UNIQUEIDENTIFIER");

        builder.HasOne(x => x.Asset)
            .WithMany()
            .HasForeignKey("AssetId")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property<Guid>("AssetId");

        builder.HasIndex("AssetId");

        builder.Property(x => x.Quantity)
        .HasColumnName("Quantity")
            .HasConversion(
                quantity => quantity.Value,
                value => new Quantity(value))
            .HasPrecision(18, 8)
            .IsRequired();

        builder.Property(x => x.ValuePerShare)
            .HasColumnName("ValuePerShare")
            .HasConversion(
                valuePerShare => valuePerShare.Value,
                value => new Money(value))
            .HasPrecision(18, 8)
            .IsRequired();

        builder.Property(x => x.Date)
            .HasColumnName("Date")
            .HasColumnType("DATETIME2")
            .IsRequired();

        builder.Ignore(x => x.Total);
    }
}