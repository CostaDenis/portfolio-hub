using PortfolioHub.Domain.Exceptions.ValueObjects;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Tests.ValueObjects;

[TestClass]
public class MoneyTests
{

    private readonly Money _money = new(20.0m);
    private readonly Quantity _quantity = new(2);

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Create_When_Money_Is_Negative()
    {
        const decimal expectedValue = -10.5m;
        Money money = new(expectedValue);

        Assert.AreEqual(expectedValue, money.Value);
    }

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Create_When_Money_Is_Valid()
    {
        const decimal expectedValue = 10.5m;
        Money money = new(expectedValue);

        Assert.AreEqual(expectedValue, money.Value);
    }

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Return_Success_When_Convert_Money_To_Decimal()
    {
        decimal result = _money;
        Assert.AreEqual(result, _money.Value);
    }

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Return_Success_When_Convert_Decimal_To_Money()
    {
        decimal result = 9.9m;
        Money money = result;
        Assert.AreEqual(money.Value, result);
    }

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Return_Success_When_Add_Is_Correct()
    {
        var newMoney = _money.Add(4.0m);
        Assert.AreEqual(24.0m, newMoney.Value);
    }

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Add_Negative_Value_Correctly()
    {
        var newMoney = _money.Add(-4.0m);
        Assert.AreEqual(16.0m, newMoney.Value);
    }

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Not_Change_Current_Instance_When_Add()
    {
        var newMoney = _money.Add(4.0m);
        Assert.AreEqual(24.0m, newMoney.Value);
        Assert.AreEqual(20.0m, _money.Value);
    }

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Return_Success_When_Subtract_Is_Correct()
    {
        var newMoney = _money.Subtract(4.0m);
        Assert.AreEqual(16.0m, newMoney.Value);
    }

    [TestMethod]
    public void Should_Return_Negative_Value_When_Subtracting_Greater_Value()
    {
        var newMoney = _money.Subtract(30.0m);
        Assert.AreEqual(-10, newMoney.Value);
    }

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Not_Change_Current_Instance_When_Subtract()
    {
        var newMoney = _money.Subtract(4.0m);
        Assert.AreEqual(16.0m, newMoney.Value);
        Assert.AreEqual(20.0m, _money.Value);
    }

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Return_Success_When_Multiply_Is_Correct()
    {
        var newMoney = _money.Multiply(_quantity);
        Assert.AreEqual(40.0m, newMoney.Value);
    }

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Not_Change_Current_Instance_When_Multiply()
    {
        var newMoney = _money.Multiply(_quantity);
        Assert.AreEqual(40.0m, newMoney.Value);
        Assert.AreEqual(20.0m, _money.Value);
    }

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Return_Exception_When_Divide_By_Zero()
        => Assert.Throws<UndeterminedResultException>(() => _money.Divide(0));

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Return_Success_When_Divide_Is_Correct()
    {
        var newMoney = _money.Divide(_quantity);
        Assert.AreEqual(10.0m, newMoney.Value);
    }

    [TestMethod]
    [TestCategory("Money Tests")]
    public void Should_Not_Change_Current_Instance_When_Divide()
    {
        var newMoney = _money.Divide(_quantity);
        Assert.AreEqual(10.0m, newMoney.Value);
        Assert.AreEqual(20.0m, _money.Value);
    }
}