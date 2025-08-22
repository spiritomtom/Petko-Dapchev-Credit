namespace CreditBank.Models;

public class Payment
{
    public Guid Id { get; set; }
    public double Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public Client Client { get; set; }
    public Credit Credit { get; set; }
}