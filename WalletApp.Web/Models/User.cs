namespace WalletApp.Web.Models
{
    public class User
    {
        public int Id { get; set; }

        public decimal CardBalance { get; set; }
        public string DailyPoints { get; set; }

        public ICollection<Transaction>? Transactions { get; set; }
    }
}
