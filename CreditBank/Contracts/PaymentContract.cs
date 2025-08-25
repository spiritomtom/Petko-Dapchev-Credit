namespace CreditBank.Contracts;

public class PaymentContract
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid CreditId { get; set; }
    public double Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
}