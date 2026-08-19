using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Infrastructure.Data.Mappings;

public class AssetMapping : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasColumnType("UNIQUEIDENTIFIER");

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasConversion(
                name => name.Value,
                value => new AssetName(value))
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(x => x.Ticker)
            .HasColumnName("Ticker")
            .HasConversion(
                ticker => ticker.Value,
                value => new Ticker(value))
            .HasMaxLength(10)
            .IsRequired();

        builder.HasIndex(x => x.Ticker)
            .IsUnique();

        builder.Property(x => x.Type)
            .HasColumnName("Type")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.OwnsOne(x => x.MarketPrice, marketPrice =>
        {
            marketPrice.Property(x => x.Price)
                .HasColumnName("MarketPrice")
                .HasConversion(
                    money => money.Value,
                    value => new Money(value))
                .HasPrecision(18, 8)
                .IsRequired();

            marketPrice.Property(x => x.LastUpdate)
                .HasColumnName("MarketPriceLastUpdate")
                .HasColumnType("DATETIME2")
                .IsRequired();
        });

        builder.Navigation(x => x.MarketPrice)
            .IsRequired();

    }
}