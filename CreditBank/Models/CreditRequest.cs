using CreditBank.Contracts.Enums;

namespace CreditBank.Models;

public class CreditRequest
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public double MonthlyIncome { get; set; }
    public double CreditAmount { get; set; }
    public CreditTypeEnum TypeEnum { get; set; }
    public CreditStatusEnum Status { get; set; } = CreditStatusEnum.Ongoing;

}