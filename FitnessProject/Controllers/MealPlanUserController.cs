using FitnessProject.Data;
using FitnessProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessProject.Controllers
{
    public class MealPlanUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MealPlanUserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /MealPlanUser/MyPlans
        public async Task<IActionResult> MyPlans()
        {
            var loggedInUser = await _userManager.GetUserAsync(User);
            if (loggedInUser == null) return Challenge(); // force login

            var plans = await _context.MealPlanTrainers
                .Include(m => m.Trainer)
                .Include(m => m.MealPlanTrainerMeals)
                    .ThenInclude(mt => mt.Meal)
                .Where(m => m.UserId == loggedInUser.Id)
                .ToListAsync();

            return View(plans);
        }

        // GET: /MealPlanUser/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var loggedInUser = await _userManager.GetUserAsync(User);
            if (loggedInUser == null) return Challenge();

            var plan = await _context.MealPlanTrainers
                .Include(m => m.Trainer)
                .Include(m => m.MealPlanTrainerMeals)
                    .ThenInclude(mt => mt.Meal)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == loggedInUser.Id);

            if (plan == null) return NotFound();

            return View(plan);
        }
    }
}
