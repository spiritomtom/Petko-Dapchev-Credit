using CreditBank.Contracts;

namespace CreditBank.Models;

public class Credit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CreditRequestId { get; set; }
    public double Amount { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public CreditTypeEnum TypeEnum { get; set; }
    public CreditStatusEnum Status { get; set; } = CreditStatusEnum.PendingReview;

    public CreditRequest CreditRequest { get; set; }
}