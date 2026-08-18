using PortfolioHub.Application.Repositories;
using PortfolioHub.Domain.Entities;

namespace PortfolioHub.Application.Tests.Repositories;

public class FakeAssetRepository : IAssetRepository
{

    public FakeAssetRepository(Asset asset)
    {
        _asset = asset;
        _assets.Add(asset);
    }

    public FakeAssetRepository(IEnumerable<Asset> assets)
    {
        _assets.AddRange(assets);
        _asset = _assets.FirstOrDefault();
    }

    public FakeAssetRepository()
    {

    }

    private Asset? _asset;
    private readonly List<Asset> _assets = [];

    public bool UpdateWasCalled { get; private set; }
    public Asset? UpdatedAsset { get; private set; }

    public Task<List<Asset>> GetAllAssets(CancellationToken cancellationToken)
        => Task.FromResult(_assets.ToList());

    public Task<Asset?> GetByIdAsync(Guid assetId, CancellationToken cancellationToken)
        => Task.FromResult(_assets.FirstOrDefault(asset => asset.Id == assetId));

    public Task UpdateAsync(Asset asset, CancellationToken cancellationToken)
    {
        UpdateWasCalled = true;
        UpdatedAsset = asset;

        return Task.CompletedTask;
    }
}
