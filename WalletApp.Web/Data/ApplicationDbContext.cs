using Microsoft.EntityFrameworkCore;

namespace WalletApp.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        
    }
}
