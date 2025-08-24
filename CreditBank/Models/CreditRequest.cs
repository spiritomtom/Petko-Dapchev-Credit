using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CreditBank.Contracts;

namespace CreditBank.Models
{
    public class CreditRequest
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public double MonthlyIncome { get; set; }
        public double CreditAmount { get; set; }
        public CreditTypeEnum CreditType { get; set; }
        public CreditStatusEnum Status { get; set; } = CreditStatusEnum.PendingReview;
        public Guid? AdministratorGuid { get; set; } = null;
        public DateTime? DateOfApproval { get; set; } = null;

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}