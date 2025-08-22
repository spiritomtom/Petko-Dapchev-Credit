using CreditBank.Contracts;
using CreditBank.Models;
using Microsoft.EntityFrameworkCore;

namespace CreditBank.Database;

public static class CreditDbContextExtension
{
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
        context.AddClientIfNotExist(creditRequest);
        var dbCreditRequest = creditRequest.ToDbCreditRequest();
        context.Credits.Add(new Credit(dbCreditRequest.Id, dbCreditRequest.CreditAmount, dbCreditRequest.TypeEnum));
        context.AddCreditRequestInternal(dbCreditRequest);
        context.SaveChanges();
    }

    private static void AddClientIfNotExist(this CreditDbContext context, CreditRequestContract creditRequest)
    {
        if (context.Clients.Find(creditRequest.Email) != null)
        {
            return;
        }

        context.Clients.Add(new Client
        {
            Id = Guid.NewGuid(),
            FullName = creditRequest.FullName,
            Email = creditRequest.Email
        });
    }

    private static void AddCreditRequestInternal(this CreditDbContext context, CreditRequest creditRequest)
    {
        var creditRequestWithSameUser = context.CreditRequests
            .FirstOrDefault(cr => cr.Email == creditRequest.Email && cr.TypeEnum == creditRequest.TypeEnum);
        if (creditRequestWithSameUser != null)
        {
            throw new InvalidOperationException($"Credit request for this user: {creditRequest.Email} of type: {creditRequest.TypeEnum} already exists.");
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
            TypeEnum = creditRequest.TypeEnum
        };
    }
}