using PortfolioHub.Domain.Entities;
using PortfolioHub.Domain.Enums;
using PortfolioHub.Domain.Exceptions;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Tests.Entities;

[TestClass]
public class TransactionTests
{

    private readonly Transaction _validTransaction;

    public TransactionTests()
    {
        _validTransaction = new Transaction(
            new Asset(new AssetName("Bitcoin"),
                        new Ticker("BTC"),
                        EAssetType.Cryptocurrency,
                        new MarketPrice(320000.0m)
                        ),
            ETransactionType.Buy,
            2,
            330000.0m
        );
    }

    [TestMethod]
    [TestCategory("Transaction Tests")]
    public void Should_Return_Exception_When_UnitPrice_Is_Negative()
    {
        Assert.Throws<InvalidPriceException>(() => new Transaction(
            new Asset(new AssetName("Bitcoin"),
                        new Ticker("BTC"),
                        EAssetType.Cryptocurrency,
                        new MarketPrice(320000.0m)
                        ),
            ETransactionType.Buy,
            1,
            -330000.0m
        ));
    }

    [TestMethod]
    [TestCategory("Transaction Tests")]
    public void Should_Create_Transaction()
    {
        Assert.IsNotNull(_validTransaction);
        Assert.AreEqual(ETransactionType.Buy, _validTransaction.Type);
        Assert.AreNotEqual(default, _validTransaction.Date);
        Assert.AreEqual(2, _validTransaction.Quantity.Value);
        Assert.AreEqual(new Money(330000.0m).Value, _validTransaction.UnitPrice.Value);
    }

    [TestMethod]
    [TestCategory("Transaction Tests")]
    public void Should_Return_Success_When_Propertie_Total_Is_Valid()
    {
        var validResult = 330000.0m * 2;
        Assert.AreEqual(validResult, _validTransaction.Total.Value);
    }

    [TestMethod]
    [TestCategory("Transaction Tests")]
    public void Should_Return_Success_When_IsBuy_Method_Is_Valid()
        => Assert.IsTrue(_validTransaction.IsBuy());

    [TestMethod]
    [TestCategory("Transaction Tests")]
    public void Should_Return_Success_When_IsSell_Method_Is_Valid()
    {
        var transaction = new Transaction(
            new Asset(new AssetName("Bitcoin"),
                        new Ticker("BTC"),
                        EAssetType.Cryptocurrency,
                        new MarketPrice(320000.0m)
                        ),
            ETransactionType.Sell,
            1,
            new Money(330000.0m)
        );

        Assert.IsTrue(transaction.IsSell());
    }

}