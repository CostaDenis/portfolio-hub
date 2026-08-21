using PortfolioHub.Domain.Exceptions.ValueObjects;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Tests.ValueObjects;

[TestClass]
public class AssetNameTests
{

    private const string _value = "Bitcoin";
    private readonly AssetName _assetName = new(_value);

    [TestMethod]
    [TestCategory("AssetName Tests")]
    public void Should_Return_Exception_When_AssetNameValue_Is_Empty()
        => Assert.Throws<InvalidAssetNameException>(() => new AssetName(""));

    [TestMethod]
    [TestCategory("AssetName Tests")]
    public void Should_Return_Exception_When_AssetNameValue_Is_WhiteSpace()
        => Assert.Throws<InvalidAssetNameException>(() => new AssetName(" "));

    [TestMethod]
    [TestCategory("AssetName Tests")]
    public void Should_Return_Exception_When_AssetNameValue_Is_Lesser_Than_Three()
        => Assert.Throws<InvalidAssetNameException>(() => new AssetName("ab"));

    [TestMethod]
    [TestCategory("AssetName Tests")]
    public void Should_Return_Exception_When_AssetNameValue_Is_Greater_Than_Sixty()
        => Assert.Throws<InvalidAssetNameException>
            (() => new AssetName("abcdefghijklmnopqrstuvwabcdefghijklmnopqrstuvwabcdefghijklmnj"));

    [TestMethod]
    [TestCategory("AssetName Tests")]
    public void Should_Create_AssetName_When_Value_Is_Valid()
        => Assert.AreEqual(_value, _assetName.Value);


    [TestMethod]
    [TestCategory("AssetName Tests")]
    public void Should_Return_Success_When_ToString_Is_Valid()
        => Assert.AreEqual(_value, _assetName.ToString());


    [TestMethod]
    [TestCategory("AssetName Tests")]
    public void Should_Convert_AssetName_To_String()
    {
        AssetName assetName = new("Wibx");
        string result = assetName;
        Assert.AreEqual(result, assetName.Value);
    }
    [TestMethod]
    [TestCategory("AssetName Tests")]
    public void Should_Convert_String_To_AssetName()
    {
        var name = "Wibx";
        AssetName assetName = name;
        Assert.AreEqual(assetName.Value, name);
    }

    [TestMethod]
    [TestCategory("AssetName Tests")]
    public void Should_Consider_Equal_Values_As_Equal()
        => Assert.AreEqual(new AssetName(_value), _assetName);

}