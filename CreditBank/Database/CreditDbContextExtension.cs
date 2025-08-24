using System.Text.RegularExpressions;
using CreditBank.Contracts;
using CreditBank.Models;
using Microsoft.EntityFrameworkCore;

namespace CreditBank.Database;

public static class CreditDbContextExtension
{
    private static readonly double MaxMorgageAmount = 500000;
    private static readonly double MaxAutoAmount = 50000;
    private static readonly double MaxPersonalAmount = 10000;
    private const string EmailPattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CreditDbContext>();
        context.Database.Migrate();
    }

    //Creates new credit request record in the database
    //If client with the same email does not exist, creates new client record as well
    //Adds the new credit record as well
    public static void AddCreditRequest(this CreditDbContext context, CreditRequestContract creditRequest)
    {
        context.AddUserIfNotExist(creditRequest);
        var dbCreditRequest = creditRequest.ToDbCreditRequest();
        context.Credits.Add(new Credit(dbCreditRequest.Id, dbCreditRequest.CreditAmount, dbCreditRequest.CreditType));
        context.AddCreditRequestInternal(dbCreditRequest);
        context.SaveChanges();
    }

    private static void AddUserIfNotExist(this CreditDbContext context, CreditRequestContract creditRequest)
    {
        if (!IsValidEmail(creditRequest.Email))
        {
            throw new ArgumentException($"Invalid email format for {creditRequest.Email}");
        }

        if (context.Users.Count(u => u.Email == creditRequest.Email) > 0)
        {
            return;
        }

        context.Users.Add(new User
        {
            Email = creditRequest.Email,
            FullName = creditRequest.FullName
        });
    }

    private static void AddCreditRequestInternal(this CreditDbContext context, CreditRequest creditRequest)
    {
        switch (creditRequest.CreditType)
        {
            case CreditTypeEnum.Auto:
                if (creditRequest.CreditAmount > MaxAutoAmount)
                {
                    throw new InvalidOperationException($"Credit amount for Auto type cannot exceed {MaxAutoAmount}.");
                }
                break;
            case CreditTypeEnum.Mortgage:
                if (creditRequest.CreditAmount > MaxMorgageAmount)
                {
                    throw new InvalidOperationException($"Credit amount for Mortgage type cannot exceed {MaxMorgageAmount}.");
                }
                break;
            case CreditTypeEnum.Personal:
                if (creditRequest.CreditAmount > MaxPersonalAmount)
                {
                    throw new InvalidOperationException($"Credit amount for Personal type cannot exceed {MaxPersonalAmount}.");
                }
                break;
            default:
                throw new InvalidOperationException($"Unknown credit type: {creditRequest.CreditType}");
        }

        if (creditRequest.MonthlyIncome <= 0)
        {
            throw new InvalidOperationException("Monthly income must be greater than zero.");
        }

        if (creditRequest.CreditAmount <= 0)
        {
            throw new InvalidOperationException("Credit amount must be greater than zero.");
        }

        var creditRequestWithSameUser = context.CreditRequests
            .FirstOrDefault(cr => cr.Email == creditRequest.Email && cr.CreditType == creditRequest.CreditType);
        if (creditRequestWithSameUser != null)
        {
            throw new InvalidOperationException($"Credit request for this user: {creditRequest.Email} of type: {creditRequest.CreditType} already exists.");
        }

        context.CreditRequests.Add(creditRequest);
        context.SaveChanges();
    }

    private static CreditRequest ToDbCreditRequest(this CreditRequestContract creditRequest)
    {
        return new CreditRequest
        {
            Id = Guid.NewGuid(),
            FullName = creditRequest.FullName,
            Email = creditRequest.Email,
            MonthlyIncome = creditRequest.MonthlyIncome,
            CreditAmount = creditRequest.CreditAmount,
            CreditType = creditRequest.TypeEnum
        };
    }

    private static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, EmailPattern);
    }
}