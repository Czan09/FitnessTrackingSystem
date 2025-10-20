using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessProject.Data;
using FitnessProject.Models;

[Authorize]
public class UserWorkoutPlanController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserWorkoutPlanController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /UserWorkoutPlan
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var plans = await _context.WorkoutPlanTrainers
            .Where(p => p.UserId == user.Id)
            .Include(p => p.Trainer)
            .Include(p => p.WorkoutPlanTrainerWorkouts)
                .ThenInclude(wptw => wptw.Workout)
            .ToListAsync();

        return View(plans);
    }

    // GET: /UserWorkoutPlan/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var plan = await _context.WorkoutPlanTrainers
            .Include(p => p.Trainer)
            .Include(p => p.WorkoutPlanTrainerWorkouts)
                .ThenInclude(wptw => wptw.Workout)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == user.Id);

        if (plan == null) return NotFound();

        return View(plan);
    }
}
