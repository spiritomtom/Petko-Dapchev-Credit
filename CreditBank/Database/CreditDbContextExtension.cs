using System.Linq;
using System.Text.RegularExpressions;
using CreditBank.Contracts;
using CreditBank.Models;
using Microsoft.EntityFrameworkCore;

namespace CreditBank.Database
{
    public static class CreditDbContextExtension
    {
        private static readonly double MaxMortgageAmount = 500000;
        private static readonly double MaxAutoAmount = 50000;
        private static readonly double MaxPersonalAmount = 10000; // Corrected to 10000
        private const string EmailPattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";

        public static void MigrateDb(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CreditDbContext>();
            context.Database.Migrate();
            Seed(context); // Ensure seed method is called
        }

        // Seed the database with an admin user
        public static void Seed(CreditDbContext context)
        {
            if (!context.Users.Any(u => u.Email == "admin@admin.com"))
            {
                context.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    Email = "admin@admin.com",
                    FullName = "Administrator",
                    Role = UserRoles.Administrator,
                });
                context.SaveChanges();
            }
        }

        // Add a new credit request, ensuring the user exists
        public static Credit AddCreditRequest(this CreditDbContext context, CreditRequestContract creditRequest)
        {
            var userId = context.FindOrCreateUser(creditRequest);

            var dbCreditRequest = creditRequest.ToDbCreditRequest(userId);

            ValidateCreditRequest(dbCreditRequest, context);

            context.CreditRequests.Add(dbCreditRequest);

            var credit = new Credit
            {
                Id = Guid.NewGuid(),
                CreditRequestId = dbCreditRequest.Id,
                Amount = dbCreditRequest.CreditAmount,
                TypeEnum = dbCreditRequest.CreditType,
                Status = CreditStatusEnum.Ongoing
            };
            context.Credits.Add(credit);

            context.SaveChanges();
            return credit;
        }

        private static Guid FindOrCreateUser(this CreditDbContext context, CreditRequestContract creditRequest)
        {
            if (!IsValidEmail(creditRequest.Email))
            {
                throw new ArgumentException($"Invalid email format for {creditRequest.Email}");
            }

            var user = context.Users.FirstOrDefault(u => u.Email == creditRequest.Email);
            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = creditRequest.FullName,
                    Email = creditRequest.Email,
                    Role = UserRoles.User
                };
                context.Users.Add(user);
                context.SaveChanges();
            }
            return user.Id;
        }

        private static void ValidateCreditRequest(CreditRequest creditRequest, CreditDbContext context)
        {
            if (creditRequest.MonthlyIncome <= 0)
            {
                throw new InvalidOperationException("Monthly income must be greater than zero.");
            }

            switch (creditRequest.CreditType)
            {
                case CreditTypeEnum.Auto:
                    if (creditRequest.CreditAmount > MaxAutoAmount)
                    {
                        throw new InvalidOperationException($"Credit amount for Auto type cannot exceed {MaxAutoAmount}.");
                    }
                    break;
                case CreditTypeEnum.Mortgage:
                    if (creditRequest.CreditAmount > MaxMortgageAmount)
                    {
                        throw new InvalidOperationException($"Credit amount for Mortgage type cannot exceed {MaxMortgageAmount}.");
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
        }

        private static CreditRequest ToDbCreditRequest(this CreditRequestContract creditRequest, Guid userId)
        {
            return new CreditRequest
            {
                Id = Guid.NewGuid(),
                FullName = creditRequest.FullName,
                Email = creditRequest.Email,
                MonthlyIncome = creditRequest.MonthlyIncome,
                CreditAmount = creditRequest.CreditAmount,
                CreditType = creditRequest.TypeEnum,
                UserId = userId
            };
        }

        private static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, EmailPattern);
        }
    }
}