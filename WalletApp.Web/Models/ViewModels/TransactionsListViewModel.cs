using WalletApp.Web.Models.Domain;

namespace WalletApp.Web.Models.ViewModels
{
    public class TransactionsListViewModel
    {
        public User User { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
