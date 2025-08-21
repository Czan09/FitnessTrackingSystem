using FitnessProject.Data;
using FitnessProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace FitnessProject.Controllers
{
    [Authorize(Roles = "Admin,Trainer")]
    public class WorkoutsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAutoTagService _autoTagService;

        public WorkoutsController(ApplicationDbContext context, IAutoTagService autoTagService)
        {
            _context = context;
            _autoTagService = autoTagService;
        }

        // GET: Workouts
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10;
            var data = _context.Workouts.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
                data = data.Where(d => d.Name.Contains(searchString));

            var totalCount = await data.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await data
                .OrderBy(d => d.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchString = searchString;

            return View(items);
        }

        // GET: Workouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var data = await _context.Workouts.FindAsync(id);
            return View(data);
        }

        // GET: Workouts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workouts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Workout workout)
        {
            if (!ModelState.IsValid)
                return View(workout);

            _context.Add(workout);
            await _context.SaveChangesAsync();

            // Call Python service to generate tags
            var tags = await _autoTagService.GetWorkoutTagsAsync(workout.Id);

            // Optionally store tags in DB
            await AddTagsToWorkout(workout.Id, tags);

            return RedirectToAction(nameof(Index));
        }

        // GET: Workouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null) return NotFound();

            return View(workout);
        }

        // POST: Workouts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Workout workout)
        {
            if (id != workout.Id) return NotFound();
            if (!ModelState.IsValid) return View(workout);

            try
            {
                _context.Update(workout);
                await _context.SaveChangesAsync();

                // Update tags after editing
                var tags = await _autoTagService.GetWorkoutTagsAsync(workout.Id);
                await UpdateTagsForWorkout(workout.Id, tags);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Workouts.Any(e => e.Id == id))
                    return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Workouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var workout = await _context.Workouts.FirstOrDefaultAsync(m => m.Id == id);
            if (workout == null) return NotFound();

            return View(workout);
        }

        // POST: Workouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workout = await _context.Workouts.FindAsync(id);
            if (workout != null)
            {
                await RemoveTagsFromWorkout(workout.Id);
                _context.Workouts.Remove(workout);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper methods to store/update/remove tags locally
        private async Task AddTagsToWorkout(int workoutId, List<string> tags)
        {
            foreach (var tagName in tags)
            {
                // Try to find the tag
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);

                // If not found, create and save it so it gets an Id
                if (tag == null)
                {
                    tag = new Tags { Name = tagName };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync();
                }

                // Check if relationship already exists
                bool exists = await _context.WorkoutTags
                    .AnyAsync(wt => wt.WorkoutId == workoutId && wt.TagId == tag.Id);

                if (!exists)
                {
                    _context.WorkoutTags.Add(new WorkoutTags
                    {
                        WorkoutId = workoutId,
                        TagId = tag.Id
                    });
                }
            }

            // Save all new WorkoutTags in one go
            await _context.SaveChangesAsync();
        }


        private async Task UpdateTagsForWorkout(int workoutId, List<string> tags)
        {
            // Remove old tags
            var oldTags = _context.WorkoutTags.Where(wt => wt.WorkoutId == workoutId);
            _context.WorkoutTags.RemoveRange(oldTags);
            await _context.SaveChangesAsync();

            // Add new tags
            await AddTagsToWorkout(workoutId, tags);
        }

        private async Task RemoveTagsFromWorkout(int workoutId)
        {
            var tags = _context.WorkoutTags.Where(wt => wt.WorkoutId == workoutId);
            _context.WorkoutTags.RemoveRange(tags);
            await _context.SaveChangesAsync();
        }
    }
}
