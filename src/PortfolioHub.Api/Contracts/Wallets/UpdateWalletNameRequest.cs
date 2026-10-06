namespace PortfolioHub.Api.Contracts.Wallets;

public class UpdateWalletNameRequest(string name)
{
    public string Name { get; init; } = name;
}