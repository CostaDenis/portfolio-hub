using PortfolioHub.Domain.Exceptions.ValueObjects;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Tests.ValueObjects;

[TestClass]
public class QuantityTests
{

    private readonly Quantity _quantity = new(1);

    [TestMethod]
    [TestCategory("Quantity Tests")]
    public void Should_Return_Exception_When_Quantity_Is_Negative()
        => Assert.Throws<InvalidQuantityException>(() => new Quantity(-1));

    [TestMethod]
    [TestCategory("Quantity Tests")]
    public void Should_Return_Success_When_Quantity_Is_Valid()
        => Assert.IsNotNull(_quantity);

    [TestMethod]
    [TestCategory("Quantity Tests")]
    public void Should_Return_Success_When_Convert_Quantity_To_Int()
    {
        decimal number = _quantity;
        Assert.AreEqual(number, _quantity.Value);
    }

    [TestMethod]
    [TestCategory("Quantity Tests")]
    public void Should_Return_Success_When_Convert_Int_To_Quantity()
    {
        var number = 1;
        Assert.AreEqual(_quantity.Value, number);
    }

    [TestMethod]
    [TestCategory("Quantity Tests")]
    public void Should_Return_Success_When_Increase_Is_Correct()
    {
        var newQuantity = _quantity.Increase(1);
        Assert.AreEqual(2, newQuantity.Value);
    }

    [TestMethod]
    [TestCategory("Quantity Tests")]
    public void Should_Not_Change_Current_Instance_When_Increasing()
    {
        var increasedQuantity = _quantity.Increase(5);
        Assert.AreEqual(1, _quantity.Value);
        Assert.AreEqual(6, increasedQuantity.Value);
    }

    [TestMethod]
    [TestCategory("Quantity Tests")]
    public void Should_Return_False_When_CanDecrease_Is_False()
    {
        var operationResult = _quantity.CanDecrease(3);
        Assert.IsFalse(operationResult);
    }

    [TestMethod]
    [TestCategory("Quantity Tests")]
    public void Should_Return_Success_When_CanDecrease_Is_Correct()
    {
        var operationResult = _quantity.CanDecrease(1);
        Assert.IsTrue(operationResult);
    }

    [TestMethod]
    [TestCategory("Quantity Tests")]
    public void Should_Return_Exception_When_Decrease_Is_Incorrect()
        => Assert.Throws<InsufficientQuantityException>(() => _quantity.Decrease(4));

    [TestMethod]
    [TestCategory("Quantity Tests")]
    public void Should_Return_Success_When_Decrease_Is_Correct()
    {
        var newQuantity = _quantity.Decrease(1);
        Assert.AreEqual(0, newQuantity.Value);
    }

    [TestMethod]
    [TestCategory("Quantity Tests")]
    public void Should_Not_Change_Current_Instance_When_Decreasing()
    {
        var decreasedQuantity = _quantity.Decrease(1);
        Assert.AreEqual(1, _quantity.Value);
        Assert.AreEqual(0, decreasedQuantity.Value);
    }

    [TestMethod]
    [TestCategory("Quantity Tests")]
    public void Should_Consider_Equal_Values_As_Equal()
        => Assert.AreEqual(new Quantity(1), _quantity);
}