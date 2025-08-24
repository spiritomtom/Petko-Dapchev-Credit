namespace CreditBank.Contracts;

public class CreditRequestContract
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public double MonthlyIncome { get; set; }
    public double CreditAmount { get; set; }
    public CreditTypeEnum TypeEnum { get; set; }
}
