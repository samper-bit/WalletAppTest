using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletApp.Web.Data;
using WalletApp.Web.Models;

namespace WalletApp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/user/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/user/{id}/balance
        [HttpGet("{id}/balance")]
        public async Task<ActionResult<decimal>> GetUserCardBalance(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.CardBalance);
        }

        // POST: api/user/{id}/points
        [HttpPost("{id}/points")]
        public async Task<ActionResult<int>> UpdateUserDailyPoints(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var dailyPoints = CalculateDailyPoints();
            user.DailyPoints = dailyPoints;
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(dailyPoints);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private string CalculateDailyPoints()
        {
            DateTime date = DateTime.Now;
            var seasonStart = GetSeasonStart(date);
            var dayInSeason = (date - seasonStart).Days + 1;

            if (dayInSeason == 1)
                return "2";
            if (dayInSeason == 2)
                return "3";

            long pointsDayBeforePrevious = 2;
            long pointsPreviousDay = 3;
            long pointsToday = 0;

            for (int day = 3; day <= dayInSeason; day++)
            {
                pointsToday = (long)Math.Round(pointsDayBeforePrevious * 1.0 + pointsPreviousDay * 0.6);

                pointsDayBeforePrevious = pointsPreviousDay;
                pointsPreviousDay = pointsToday;
            }

            if (pointsToday >= 1_000_000_000)
            {
                return $"{pointsToday / 1_000_000_000.0:F0}B"; // Округлення до мільярдів
            }
            else if (pointsToday >= 1_000_000)
            {
                return $"{pointsToday / 1_000_000.0:F0}M"; // Округлення до мільйонів
            }
            else if (pointsToday >= 1_000)
            {
                return $"{pointsToday / 1_000.0:F0}K"; // Округлення до тисяч
            }
            return pointsToday.ToString();
        }

        private DateTime GetSeasonStart(DateTime date)
        {
            int year = date.Year;
            if (date >= new DateTime(year, 12, 1) || date < new DateTime(year, 3, 1))
            {
                // Зима
                return new DateTime(year, 12, 1);
            }
            else if (date >= new DateTime(year, 3, 1) && date < new DateTime(year, 6, 1))
            {
                // Весна
                return new DateTime(year, 3, 1);
            }
            else if (date >= new DateTime(year, 6, 1) && date < new DateTime(year, 9, 1))
            {
                // Літо
                return new DateTime(year, 6, 1);
            }
            else
            {
                // Осінь
                return new DateTime(year, 9, 1);
            }
        }
    }
}
