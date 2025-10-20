using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessProject.Data;
using FitnessProject.Models;

[Authorize(Roles = "Trainer")]
public class MealPlanTrainerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MealPlanTrainerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: MealPlanTrainer
    public async Task<IActionResult> Index()
    {
        var plans = await _context.MealPlanTrainers
            .Include(p => p.User)
            .Include(p => p.Trainer)
            .Include(p => p.MealPlanTrainerMeals)
                .ThenInclude(mp => mp.Meal)
            .ToListAsync();
        return View(plans);
    }

    // GET: Create
    // GET: Create
    public async Task<IActionResult> Create()
    {
        ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName");

        var diets = await _context.Diets.ToListAsync();

        // prevent null by always initializing
        ViewBag.Meals = new MultiSelectList(diets ?? new List<Diets>(), "Id", "MealName");

        return View();
    }

    // POST: Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MealPlanTrainer plan, int[] selectedMeals)
    {
        var loggedInTrainer = await _userManager.GetUserAsync(User);
        var trainerDetails = await _context.TrainerDetails
            .FirstOrDefaultAsync(t => t.ApplicationUserId == loggedInTrainer.Id);

        if (trainerDetails == null)
            return BadRequest("Trainer profile not found.");

        plan.TrainerId = trainerDetails.Id;

        if (ModelState.IsValid)
        {
            // Associate selected diets
            plan.MealPlanTrainerMeals = selectedMeals
                .Select(id => new MealPlanTrainerMeal { MealId = id })
                .ToList();

            _context.Add(plan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Repopulate dropdowns if ModelState invalid
        ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName", plan.UserId);
        var diets = await _context.Diets.ToListAsync();
        ViewBag.Meals = new MultiSelectList(diets ?? new List<Diets>(), "Id", "MealName", selectedMeals);

        return View(plan);
    }

    // GET: Edit
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var plan = await _context.MealPlanTrainers
            .Include(p => p.MealPlanTrainerMeals)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (plan == null) return NotFound();

        ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName", plan.UserId);
        ViewBag.Meals = new MultiSelectList(_context.MealPlanMeals, "Id", "Name", plan.MealPlanTrainerMeals.Select(m => m.MealId));
        return View(plan);
    }

    // POST: Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MealPlanTrainer plan, int[] selectedMeals)
    {
        if (id != plan.Id) return NotFound();

        var dbPlan = await _context.MealPlanTrainers
            .Include(p => p.MealPlanTrainerMeals)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (dbPlan == null) return NotFound();

        dbPlan.PlanName = plan.PlanName;
        dbPlan.UserId = plan.UserId;

        // Update meals
        dbPlan.MealPlanTrainerMeals.Clear();
        dbPlan.MealPlanTrainerMeals = selectedMeals.Select(mid => new MealPlanTrainerMeal { MealId = mid }).ToList();

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: MealPlanTrainer/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var mealPlan = await _context.MealPlanTrainers
            .Include(mp => mp.User)
            .Include(mp => mp.Trainer)
            .Include(mp => mp.MealPlanTrainerMeals)
                .ThenInclude(mptm => mptm.Meal)
            .FirstOrDefaultAsync(mp => mp.Id == id);

        if (mealPlan == null) return NotFound();

        return View(mealPlan);
    }

    // GET: MealPlanTrainer/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var mealPlan = await _context.MealPlanTrainers
            .Include(mp => mp.User)
            .Include(mp => mp.Trainer)
            .Include(mp => mp.MealPlanTrainerMeals)
                .ThenInclude(mptm => mptm.Meal)
            .FirstOrDefaultAsync(mp => mp.Id == id);

        if (mealPlan == null) return NotFound();

        // Only creator trainer can delete
        var loggedInTrainer = await _userManager.GetUserAsync(User);
        var trainerDetails = await _context.TrainerDetails
            .FirstOrDefaultAsync(t => t.ApplicationUserId == loggedInTrainer.Id);

        if (mealPlan.TrainerId != trainerDetails?.Id) return Forbid();

        return View(mealPlan);
    }

    // POST: MealPlanTrainer/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var mealPlan = await _context.MealPlanTrainers
            .Include(mp => mp.MealPlanTrainerMeals)
            .FirstOrDefaultAsync(mp => mp.Id == id);

        if (mealPlan == null) return NotFound();

        var loggedInTrainer = await _userManager.GetUserAsync(User);
        var trainerDetails = await _context.TrainerDetails
            .FirstOrDefaultAsync(t => t.ApplicationUserId == loggedInTrainer.Id);

        if (mealPlan.TrainerId != trainerDetails?.Id) return Forbid();

        // Remove associated join table records first
        if (mealPlan.MealPlanTrainerMeals != null && mealPlan.MealPlanTrainerMeals.Any())
        {
            _context.MealPlanTrainerMeals.RemoveRange(mealPlan.MealPlanTrainerMeals);
        }

        _context.MealPlanTrainers.Remove(mealPlan);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

}
