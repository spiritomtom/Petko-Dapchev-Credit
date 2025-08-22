namespace CreditBank.Models;

public class Client
{
    public string Email { get; set; }

    public Guid Id { get; set; }
    public string FullName { get; set; }

    public List<CreditRequest> CreditRequests { get; set; } = new();
    public List<Payment> Payments { get; set; } = new();
}