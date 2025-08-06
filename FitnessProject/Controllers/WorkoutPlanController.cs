using FitnessProject.Data;
using FitnessProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessProject.Controllers
{
    [Authorize(Roles = "Admin,Trainer")]
    public class WorkoutPlanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkoutPlanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkoutPlan
        public async Task<IActionResult> Index()
        {
            var plans = _context.WorkoutPlans.Include(wp => wp.Trainer);
            return View(await plans.ToListAsync());
        }

        // GET: WorkoutPlan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var workoutPlan = await _context.WorkoutPlans
                .Include(wp => wp.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workoutPlan == null) return NotFound();

            return View(workoutPlan);
        }

        // GET: WorkoutPlan/Create
        public IActionResult Create()
        {
            ViewData["TrainerId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.TrainerDetails, "Id", "Name");
            return View();
        }

        // POST: WorkoutPlan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutPlan workoutPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workoutPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TrainerId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.TrainerDetails, "Id", "Name", workoutPlan.TrainerId);
            return View(workoutPlan);
        }

        // GET: WorkoutPlan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var workoutPlan = await _context.WorkoutPlans.FindAsync(id);
            if (workoutPlan == null) return NotFound();

            ViewData["TrainerId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.TrainerDetails, "Id", "Name", workoutPlan.TrainerId);
            return View(workoutPlan);
        }

        // POST: WorkoutPlan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkoutPlan workoutPlan)
        {
            if (id != workoutPlan.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workoutPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.WorkoutPlans.Any(e => e.Id == id))
                        return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TrainerId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.TrainerDetails, "Id", "Name", workoutPlan.TrainerId);
            return View(workoutPlan);
        }

        // GET: WorkoutPlan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var workoutPlan = await _context.WorkoutPlans
                .Include(wp => wp.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workoutPlan == null) return NotFound();

            return View(workoutPlan);
        }

        // POST: WorkoutPlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workoutPlan = await _context.WorkoutPlans.FindAsync(id);
            _context.WorkoutPlans.Remove(workoutPlan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
