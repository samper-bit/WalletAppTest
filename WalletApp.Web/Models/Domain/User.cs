using System.ComponentModel.DataAnnotations;

namespace WalletApp.Web.Models.Domain
{
    public class User
    {
        public int Id { get; set; }

        [Range(0, 1500, ErrorMessage = "Card balance must be between 0 and 1500.")]
        public decimal CardBalance { get; set; }
        public string DailyPoints { get; set; }

        public ICollection<Transaction>? Transactions { get; set; }
    }
}
