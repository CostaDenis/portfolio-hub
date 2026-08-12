using PortfolioHub.Domain.Exceptions.ValueObjects;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Tests.ValueObjects;

[TestClass]
public class WalletNameTests
{
    [TestMethod]
    [TestCategory("WalletName Tests")]
    public void Should_Return_Exception_When_WalletName_Is_Empty()
        => Assert.Throws<InvalidWalletNameException>(() => new WalletName(""));

    [TestMethod]
    [TestCategory("WalletName Tests")]
    public void Should_Return_Exception_When_WalletName_Is_Lesser_Than_Two()
        => Assert.Throws<InvalidWalletNameException>(() => new WalletName("D"));

    [TestMethod]
    [TestCategory("WalletName Tests")]
    public void Should_Return_Exception_When_WalletName_Is_Greater_Than_Twenty()
        => Assert.Throws<InvalidWalletNameException>(() => new WalletName("ABCDEFGHIJKLMNOPQRSTU"));

    [TestMethod]
    [TestCategory("WalletName Tests")]
    public void Should_Create_When_WalletName_Is_Valid()
    {
        WalletName walletName = new("FIIs");
        Assert.IsNotNull(walletName.Value);
    }

    [TestMethod]
    [TestCategory("WalletName Tests")]
    public void Should_Return_Success_When_ToString_Is_Valid()
        => Assert.AreEqual("FIIs", new WalletName("FIIs").ToString());

    [TestMethod]
    [TestCategory("WalletName Tests")]
    public void Should_Return_Success_When_Convert_WalletName_To_String()
    {
        WalletName walletName = new("FIIs");
        string result = walletName;
        Assert.AreEqual("FIIs", result);
    }

    [TestMethod]
    [TestCategory("WalletName Tests")]
    public void Should_Return_Success_When_Convert_String_To_WalletName()
    {
        const string value = "FIIs";
        WalletName walletName = value;

        Assert.AreEqual(value, walletName.Value);
    }

}