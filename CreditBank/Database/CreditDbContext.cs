using CreditBank.Models;
using Microsoft.EntityFrameworkCore;

namespace CreditBank.Database;

public class CreditDbContext(DbContextOptions options) 
    : DbContext (options)
{
    internal DbSet<CreditRequest> CreditRequests => Set<CreditRequest>();
    internal DbSet<Client> Clients => Set<Client>();
    internal DbSet<Credit> Credits => Set<Credit>();

}

