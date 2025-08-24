namespace CreditBank.Contracts;

public class PaymentContract
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CreditId { get; set; }
    public double Amount { get; set; }
    public DateTime PaymentDate { get; set; }
}