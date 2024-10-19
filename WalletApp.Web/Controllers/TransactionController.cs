using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletApp.Web.Data;
using WalletApp.Web.Models.Domain;
using WalletApp.Web.Models.ViewModels;


namespace WalletApp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> TransactionsList(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Transactions)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var transactions = user.Transactions
                .OrderByDescending(t => t.Date)
                .Take(10)
                .ToList();

            var viewModel = new TransactionsListViewModel
            {
                User = user,
                Transactions = transactions
            };

            return View(viewModel);
        }

        // GET: api/transaction
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions(int? userId)
        {
            IQueryable<Transaction> query = _context.Transactions;

            if (userId.HasValue)
            {
                query = query.Where(t => t.UserId == userId);
            }

            var transactions = await query.ToListAsync();
            return Ok(transactions);
        }

        // GET: api/transaction/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound("Transaction not found!");
            }

            return Ok(transaction);
        }

        // POST: api/transaction
        [HttpPost]
        public async Task<ActionResult<Transaction>> Create(Transaction transaction)
        {
            if (!Enum.IsDefined(typeof(TransactionType), transaction.Type))
                return BadRequest("Invalid transaction type!");

            if (transaction.Title.Length == 0)
                return BadRequest("Title can't be empty!");

            transaction.Icon = FormatHexColor(transaction.Icon);

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }

        // PUT: api/transaction/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Transaction updatedTransaction)
        {
            if (id != updatedTransaction.Id)
            {
                return BadRequest("TransactionIdMismatch");
            }

            if (!Enum.IsDefined(typeof(TransactionType), updatedTransaction.Type))
                return BadRequest("Invalid transaction type!");

            if (updatedTransaction.Title.Length == 0)
                return BadRequest("Title can't be empty!");

            updatedTransaction.Icon = FormatHexColor(updatedTransaction.Icon);

            _context.Entry(updatedTransaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound("Transaction not found!");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/transactions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound("Transaction not found!");
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }

        private string FormatHexColor(string icon)
        {
            Regex hexWithoutHashtag = new Regex("^(([0-7][0-9A-Fa-f]){3}|([0-7][0-9A-Fa-f]){6})$");
            Regex hexWithHashtag = new Regex("^#(([0-7][0-9A-Fa-f]){3}|([0-7][0-9A-Fa-f]){6})$");

            if (hexWithoutHashtag.IsMatch(icon))
            {
                return $"#{icon}";
            }
            else if (!hexWithHashtag.IsMatch(icon))
            {
                Random random = new Random();
                return $"#{random.Next(0, 128):X2}{random.Next(0, 128):X2}{random.Next(0, 128):X2}";
            }
            return icon;
        }
    }
}
