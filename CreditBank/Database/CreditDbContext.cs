using CreditBank.Contracts;
using CreditBank.Models;
using Microsoft.EntityFrameworkCore;

namespace CreditBank.Database;

public class CreditDbContext : DbContext
{
    public CreditDbContext(DbContextOptions<CreditDbContext> options) : base(options)
    {
    }

    public DbSet<CreditRequest> CreditRequests { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Credit> Credits { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<CreditRequest>()
            .HasKey(cr => cr.Id);

        modelBuilder.Entity<CreditRequest>()
            .HasOne(cr => cr.User)
            .WithMany(u => u.CreditRequests)
            .HasForeignKey(cr => cr.UserId)
            .HasPrincipalKey(u => u.Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Credit>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Payment>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.User)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.UserId)
            .HasPrincipalKey(u => u.Id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Credit)
            .WithMany()
            .HasForeignKey(p => p.CreditId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public static void Seed(CreditDbContext context)
    {
        if (context.Users.Any(u => u.Role == UserRoles.Administrator)) return;
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