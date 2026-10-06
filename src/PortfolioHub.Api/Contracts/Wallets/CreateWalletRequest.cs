namespace PortfolioHub.Api.Contracts.Wallets;

public class CreateWalletRequest(string name)
{
    public string Name { get; init; } = name;
}