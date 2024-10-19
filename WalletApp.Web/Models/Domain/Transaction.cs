using System.ComponentModel.DataAnnotations;

namespace WalletApp.Web.Models.Domain
{
    public class Transaction
    {
        public int Id { get; set; }

        [EnumDataType(typeof(TransactionType), ErrorMessage = "Invalid transaction type.")]
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }

        [Required]
        public string Title { get; set; }

        [MaxLength(100)]
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public bool Pending { get; set; }

        [MaxLength(40)]
        public string? AuthorizedUser { get; set; }
        public string? Icon { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}

public enum TransactionType
{
    Payment,
    Credit
}