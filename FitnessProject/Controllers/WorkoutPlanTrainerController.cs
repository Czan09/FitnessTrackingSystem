using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FitnessProject.Data;
using FitnessProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class WorkoutPlanTrainerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public WorkoutPlanTrainerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: WorkoutPlanTrainer
    public async Task<IActionResult> Index()
    {
        var plans = await _context.WorkoutPlanTrainers
            .Include(p => p.User)
            .Include(p => p.Trainer)
            .ToListAsync();
        return View(plans);
    }

    // GET: WorkoutPlanTrainer/Create
    public async Task<IActionResult> Create()
    {
        var loggedInTrainer = await _userManager.GetUserAsync(User);
        if (loggedInTrainer != null)
            ViewBag.TrainerId = loggedInTrainer.Id;

        ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName");
        ViewBag.Workouts = new MultiSelectList(_context.Workouts, "Id", "Name");
        return View();
    }

    // POST: WorkoutPlanTrainer/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkoutPlanTrainer plan)
    {
        var loggedInTrainer = await _userManager.GetUserAsync(User);
        if (loggedInTrainer == null) return Unauthorized();

        var trainerDetails = await _context.TrainerDetails
            .FirstOrDefaultAsync(t => t.ApplicationUserId == loggedInTrainer.Id);

        if (trainerDetails == null) return BadRequest("Trainer profile not found.");

        // ASSIGN server-side
        plan.TrainerId = trainerDetails.Id;

        // Remove ModelState entry for TrainerId to avoid validation issues
        ModelState.Remove(nameof(plan.TrainerId));

        // Map selected workouts
        plan.WorkoutPlanTrainerWorkouts = plan.WorkoutIds
            .Select(id => new WorkoutPlanTrainerWorkout { WorkoutId = id })
            .ToList();

        if (ModelState.IsValid)
        {
            _context.Add(plan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Repopulate dropdowns/checklists if validation fails
        ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName", plan.UserId);
        ViewBag.Workouts = new MultiSelectList(_context.Workouts, "Id", "Name", plan.WorkoutIds);

        return View(plan);
    }

    // GET: WorkoutPlanTrainer/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var plan = await _context.WorkoutPlanTrainers
            .Include(p => p.Trainer)
            .Include(p => p.WorkoutPlanTrainerWorkouts)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plan == null) return NotFound();

        // Only creator can edit
        var loggedInTrainer = await _userManager.GetUserAsync(User);
        var trainerDetails = await _context.TrainerDetails
            .FirstOrDefaultAsync(t => t.ApplicationUserId == loggedInTrainer.Id);

        if (plan.TrainerId != trainerDetails?.Id) return Forbid();

        // Pass Users dropdown
        ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName", plan.UserId);

        // Pass Workouts for checkbox list
        var selectedWorkoutIds = plan.WorkoutPlanTrainerWorkouts.Select(w => w.WorkoutId).ToList();
        ViewBag.Workouts = new MultiSelectList(_context.Workouts, "Id", "Name", selectedWorkoutIds);

        return View(plan);
    }

    // POST: WorkoutPlanTrainer/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WorkoutPlanTrainer plan)
    {
        if (id != plan.Id) return NotFound();

        var dbPlan = await _context.WorkoutPlanTrainers
            .Include(p => p.WorkoutPlanTrainerWorkouts)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (dbPlan == null) return NotFound();

        var loggedInTrainer = await _userManager.GetUserAsync(User);
        var trainerDetails = await _context.TrainerDetails
            .FirstOrDefaultAsync(t => t.ApplicationUserId == loggedInTrainer.Id);

        if (dbPlan.TrainerId != trainerDetails?.Id) return Forbid();

        // Set TrainerId server-side to prevent tampering
        plan.TrainerId = dbPlan.TrainerId;
        ModelState.Remove(nameof(plan.TrainerId));

        if (ModelState.IsValid)
        {
            dbPlan.PlanName = plan.PlanName;
            dbPlan.UserId = plan.UserId;

            // Update selected workouts
            dbPlan.WorkoutPlanTrainerWorkouts.Clear();
            if (plan.WorkoutIds != null && plan.WorkoutIds.Any())
            {
                dbPlan.WorkoutPlanTrainerWorkouts = plan.WorkoutIds
                    .Select(wId => new WorkoutPlanTrainerWorkout
                    {
                        WorkoutPlanTrainerId = dbPlan.Id,
                        WorkoutId = wId
                    }).ToList();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Repopulate dropdowns/checkboxes if validation fails
        ViewBag.UserId = new SelectList(_context.Users, "Id", "UserName", plan.UserId);
        ViewBag.Workouts = new MultiSelectList(_context.Workouts, "Id", "Name", plan.WorkoutIds);

        return View(plan);
    }

    // GET: Details
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var plan = await _context.WorkoutPlanTrainers
            .Include(p => p.User)
            .Include(p => p.Trainer)
            .Include(p => p.WorkoutPlanTrainerWorkouts)
                .ThenInclude(wpt => wpt.Workout)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plan == null) return NotFound();

        return View(plan);
    }

    // GET: Delete
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var plan = await _context.WorkoutPlanTrainers
            .Include(p => p.User)
            .Include(p => p.Trainer)
            .Include(p => p.WorkoutPlanTrainerWorkouts)
                .ThenInclude(wpt => wpt.Workout)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plan == null) return NotFound();

        var loggedInTrainer = await _userManager.GetUserAsync(User);
        var trainer = await _context.TrainerDetails.FirstOrDefaultAsync(t => t.ApplicationUserId == loggedInTrainer.Id);
        if (plan.TrainerId != trainer?.Id) return Forbid();

        return View(plan);
    }

    // POST: Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var plan = await _context.WorkoutPlanTrainers.FindAsync(id);
        if (plan == null) return NotFound();

        var loggedInTrainer = await _userManager.GetUserAsync(User);
        var trainer = await _context.TrainerDetails.FirstOrDefaultAsync(t => t.ApplicationUserId == loggedInTrainer.Id);
        if (plan.TrainerId != trainer?.Id) return Forbid();

        _context.WorkoutPlanTrainers.Remove(plan);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}