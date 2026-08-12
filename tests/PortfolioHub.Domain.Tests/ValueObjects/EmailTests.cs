using PortfolioHub.Domain.Exceptions.ValueObjects;
using PortfolioHub.Domain.ValueObjects;

namespace PortfolioHub.Domain.Tests.ValueObjects;

[TestClass]
public class EmailTests
{
    private readonly Email _emailValid = new("denis@gmail.com");

    [TestMethod]
    [TestCategory("Email Tests")]
    public void Should_Return_Exception_When_Email_Is_Empty()
        => Assert.Throws<InvalidEmailException>(() => new Email(""));

    [TestMethod]
    [TestCategory("Email Tests")]
    public void Should_Return_Exception_When_Email_Is_Invalid()
        => Assert.Throws<InvalidEmailException>(() => new Email("teclado3254"));


    [TestMethod]
    [TestCategory("Email Tests")]
    public void Should_Create_Email_When_Value_Is_Valid()
    {
        Email email = new("deniscosta@gmail.com");
        Assert.IsNotNull(email.Value);
    }

    [TestMethod]
    [TestCategory("Email Tests")]
    public void Should_Return_Success_When_ToString_Is_Valid()
        => Assert.AreEqual("denis@gmail.com", _emailValid.ToString());

    [TestMethod]
    [TestCategory("Email Tests")]
    public void Should_Return_Success_When_Convert_Email_To_String()
    {
        string result = _emailValid;
        Assert.AreEqual("denis@gmail.com", result);
    }

    [TestMethod]
    [TestCategory("Email Tests")]
    public void Should_Return_Success_When_Convert_String_To_Email()
    {
        const string value = "denis@gmail.com";
        Email email = value;

        Assert.AreEqual(value, email.Value);
    }
}