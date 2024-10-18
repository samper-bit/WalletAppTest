using System.ComponentModel.DataAnnotations;

namespace WalletApp.Web.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }

        [Required]
        public string Title { get; set; }

        [MaxLength(100)]
        public string? Description { get; set; }
        public DateTime Data { get; set; }
        public bool Pending { get; set; }

        [MaxLength(40)]
        public string? AuthorizedUser { get; set; }
        public string Icon { get; set; }

        public int UserId {get; set; }
        public User User { get; set; }
    }
}

public enum TransactionType
{
    Payment,
    Credit
}