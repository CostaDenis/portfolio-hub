using PortfolioHub.Domain.ValueObjects;
using PortfolioHub.Domain.ValueObjects.Exceptions;

namespace PortfolioHub.Domain.Tests.ValueObjects;

[TestClass]
public class TickerTests
{
    private readonly Ticker _ticker = new Ticker("MXRF11");

    [TestMethod]
    [TestCategory("Ticker Tests")]
    public void Should_Return_Exception_When_Ticker_Is_Empty()
        => Assert.Throws<InvalidTickerException>(() => new Ticker(""));
    
    [TestMethod]
    [TestCategory("Ticker Tests")]
    public void Should_Return_Exception_When_Ticker_Is_Lesser_Than_Two()
        => Assert.Throws<InvalidTickerException>(() => new Ticker("a"));
    
    [TestMethod]
    [TestCategory("Ticker Tests")]
    public void Should_Return_Exception_When_Ticker_Is_Greater_Than_Ten()
        => Assert.Throws<InvalidTickerException>(() => new Ticker("12345abcdef"));
    
    [TestMethod]
    [TestCategory("Ticker Tests")]
    public void Should_Return_Exception_When_Ticker_Is_Invalid()
        => Assert.Throws<InvalidTickerException>(() => new Ticker("AAS*123"));

    [TestMethod]
    [TestCategory("Ticker Tests")]
    public void Should_Return_Success_When_Ticker_Is_Valid()
    {
        var ticker = new Ticker("XPML11");
        Assert.AreEqual("XPML11", ticker.Value);
    }
    
    [TestMethod]
    [TestCategory("Ticker Tests")]
    public void Should_Normalize_Ticker_To_Uppercase()
    {
        var ticker = new Ticker("xpml11");
        Assert.AreEqual("XPML11", ticker.Value);
    }
    
    [TestMethod]
    [TestCategory("Ticker Tests")]
    public void Should_Trim_WhiteSpaces()
    {
        var ticker = new Ticker(" xpml11 ");
        Assert.AreEqual("XPML11", ticker.Value);
    }
    
    [TestMethod]
    [TestCategory("Ticker Tests")]
    public void Should_Return_Success_When_ToString_Is_Valid()
        => Assert.AreEqual("MXRF11", _ticker.ToString());
    
    [TestMethod]
    [TestCategory("Ticker Tests")]
    public void Should_Return_Success_When_Convert_Ticker_To_String()
    {
        string result = _ticker;
        Assert.AreEqual("MXRF11", result);
    }
    
    [TestMethod]
    [TestCategory("Ticker Tests")]
    public void Should_Return_Success_When_Convert_String_To_Ticker()
    {
        const string value = "BTCI11";
        Ticker ticker = value;
        
        Assert.AreEqual(value, ticker.Value);
    }
}