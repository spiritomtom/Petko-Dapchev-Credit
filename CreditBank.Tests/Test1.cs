using CreditBank.Contracts;
using CreditBank.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CreditBank.Tests;

public class CreditDbContextExtensionTests
{
    private readonly CreditDbContext _context;

    public CreditDbContextExtensionTests()
    {
        var options = new DbContextOptionsBuilder<CreditDbContext>()
            .UseInMemoryDatabase(databaseName: "CreditBankTestDb")
            .Options;

        _context = new CreditDbContext(options);
    }

    [Fact]
    public void AddCreditRequest_Should_Create_User_If_Not_Exist()
    {
        // Arrange
        var request = new CreditRequestContract
        {
            FullName = "Pesho Peshev",
            Email = "pesho.peshev@gmail.com",
            MonthlyIncome = 3000,
            CreditAmount = 5000,
            TypeEnum = CreditTypeEnum.Personal
        };

        // Act
        _context.AddCreditRequest(request);
        var userExists = _context.Users.Any(u => u.Email == "pesho.peshev@gmail.com");
        var creditRequestExists = _context.CreditRequests.Any(cr => cr.Email == "pesho.peshev@gmail.com");

        // Assert
        Assert.IsTrue(userExists);
        Assert.IsTrue(creditRequestExists);
    }

    [Fact]
    public void AddCreditRequest_Should_Throw_On_Invalid_Email()
    {
        // Arrange
        var request = new CreditRequestContract
        {
            FullName = "Invalid Email User",
            Email = "invalid-email",
            MonthlyIncome = 3000,
            CreditAmount = 5000,
            TypeEnum = CreditTypeEnum.Personal
        };

        // Act & Assert
        var ex = Assert.ThrowsException<ArgumentException>(() => _context.AddCreditRequest(request));
        Assert.Equals("Invalid email format for invalid-email", ex.Message);
    }

    [Fact]
    public void AddCreditRequest_Should_Throw_On_Negative_CreditAmount()
    {
        // Arrange
        var request = new CreditRequestContract
        {
            FullName = "Pesho Peshev",
            Email = "pesho.peshev@gmail.com",
            MonthlyIncome = 3000,
            CreditAmount = -100,
            TypeEnum = CreditTypeEnum.Personal
        };

        // Act & Assert
        var ex = Assert.ThrowsException<InvalidOperationException>(() => _context.AddCreditRequest(request));
        Assert.Equals("Credit amount must be greater than zero.", ex.Message);
    }

    [Fact]
    public void AddCreditRequest_Should_Throw_On_Exceeding_Limit()
    {
        // Arrange
        var request = new CreditRequestContract
        {
            FullName = "Pesho Peshev",
            Email = "pesho.peshev@gmail.com",
            MonthlyIncome = 3000,
            CreditAmount = 60000,
            TypeEnum = CreditTypeEnum.Auto
        };

        // Act & Assert
        var ex = Assert.ThrowsException<InvalidOperationException>(() => _context.AddCreditRequest(request));
        Assert.Equals("Credit amount for Auto type cannot exceed 50000.", ex.Message);
    }

    [Fact]
    public void AddCreditRequest_Should_Throw_If_Request_Already_Exists()
    {
        // Arrange
        var request = new CreditRequestContract
        {
            FullName = "Pesho Peshev",
            Email = "pesho.peshev@gmail.com",
            MonthlyIncome = 3000,
            CreditAmount = 5000,
            TypeEnum = CreditTypeEnum.Personal
        };

        _context.AddCreditRequest(request);

        var ex = Assert.ThrowsException<InvalidOperationException>(() => _context.AddCreditRequest(request));
        Assert.Equals("Credit request for this user: jane.doe@example.com of type: Personal already exists.", ex.Message);
    }

    [Fact]
    public void AddCreditRequest_Should_Not_Create_Duplicate_User()
    {
        // Arrange
        var request1 = new CreditRequestContract
        {
            FullName = "Pesho Peshev",
            Email = "pesho.peshev@example.com",
            MonthlyIncome = 3000,
            CreditAmount = 5000,
            TypeEnum = CreditTypeEnum.Personal
        };

        var request2 = new CreditRequestContract
        {
            FullName = "John Doe",
            Email = "pesho.peshev@gmail.com",
            MonthlyIncome = 4000,
            CreditAmount = 10000,
            TypeEnum = CreditTypeEnum.Auto
        };

        _context.AddCreditRequest(request1);
        _context.AddCreditRequest(request2);

        var userCount = _context.Users.Count(u => u.Email == "pesho.peshev@gmail.com");
        Assert.Equals(1, userCount);
    }
}