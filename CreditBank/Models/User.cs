using System.ComponentModel.DataAnnotations;
using CreditBank.Contracts;

namespace CreditBank.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string Email { get; set; }

    public string FullName { get; set; }
    public UserRoles Role { get; set; } = UserRoles.User;

    public List<CreditRequest> CreditRequests { get; set; } = [];
    public List<Payment> Payments { get; set; } = [];
}