using CreditBank.Contracts.Enums;

namespace CreditBank.Models;

public class Credit(Guid creditRequestId, double amount, CreditTypeEnum typeEnum)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CreditRequestId { get; set; } = creditRequestId;
    public double Amount { get; set; } = amount;
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; } = null;
    public CreditTypeEnum TypeEnum { get; set; } = typeEnum;
    public CreditStatusEnum Status { get; set; } = CreditStatusEnum.Ongoing;
    public CreditRequest CreditRequest { get; }
}