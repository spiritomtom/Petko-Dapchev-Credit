using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditBank.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public Guid CreditId { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("CreditId")]
        public Credit Credit { get; set; }
    }
}