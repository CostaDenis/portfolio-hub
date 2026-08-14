namespace PortfolioHub.Application.Queries.Assets;

public class GetAssetByIdQuery(Guid assetId)
{
    public Guid AssetId { get; init; } = assetId;
}