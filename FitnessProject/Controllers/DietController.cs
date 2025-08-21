using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessProject.Data;
using FitnessProject.Models;
using FitnessProject.Models.enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FitnessProject.Controllers
{
    [Authorize(Roles = "Admin,Trainer")]
    public class DietController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAutoTagService _autoTagService;

        public DietController(ApplicationDbContext context, IAutoTagService autoTagService)
        {
            _context = context;
            _autoTagService = autoTagService;
        }

        // GET: Diet
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10;
            var diets = _context.Diets.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                diets = diets.Where(d => d.MealName.Contains(searchString));

            var totalCount = await diets.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await diets
                .OrderBy(d => d.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchString = searchString;

            return View(items);
        }

        // GET: Diet/Create
        public IActionResult Create()
        {
            ViewData["DietTypes"] = new SelectList(Enum.GetValues(typeof(DietType)));
            return View();
        }

        // POST: Diet/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Diets diet)
        {
            if (!ModelState.IsValid)
            {
                ViewData["DietTypes"] = new SelectList(Enum.GetValues(typeof(DietType)), diet.Type);
                return View(diet);
            }

            _context.Add(diet);

            await _context.SaveChangesAsync();
            
            var tags = await _autoTagService.GetMealTagsAsync(diet.Id);

            await AddTagsToDiet(diet.Id, tags);

            return RedirectToAction(nameof(Index));
        }

        // GET: Diet/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var diet = await _context.Diets.FindAsync(id);
            if (diet == null) return NotFound();

            ViewData["DietTypes"] = new SelectList(Enum.GetValues(typeof(DietType)), diet.Type);
            return View(diet);
        }

        // POST: Diet/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Diets diet)
        {
            if (id != diet.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["DietTypes"] = new SelectList(Enum.GetValues(typeof(DietType)), diet.Type);
                return View(diet);
            }

            try
            {
                _context.Update(diet);
                await _context.SaveChangesAsync();

                // Update tags after editing
                var tags = await _autoTagService.GetMealTagsAsync(diet.Id);
                await UpdateTagsForDiet(diet.Id, tags);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Diets.Any(e => e.Id == id))
                    return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Diet/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var diet = await _context.Diets.FirstOrDefaultAsync(m => m.Id == id);
            if (diet == null) return NotFound();

            return View(diet);
        }

        // POST: Diet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diet = await _context.Diets.FindAsync(id);
            if (diet != null)
            {
                // Remove associated tags if you store them
                await RemoveTagsFromDiet(id);

                _context.Diets.Remove(diet);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Optional helper methods for storing tags locally
        private async Task AddTagsToDiet(int dietId, List<string> tags)
        {
            foreach (var tagName in tags)
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (tag == null)
                {
                    tag = new Tags { Name = tagName };
                    _context.Tags.Add(tag);

                    await _context.SaveChangesAsync();
                }

                var exists = await _context.MealTags
                    .AnyAsync(mt => mt.MealId == dietId && mt.TagId == tag.Id);

                if (!exists)
                {
                    _context.MealTags.Add(new MealTags
                    {
                        MealId = dietId,
                        TagId = tag.Id
                    });
                }
            }

            await _context.SaveChangesAsync();
        }


        private async Task UpdateTagsForDiet(int dietId, List<string> tags)
        {
            // Remove old tags
            var oldTags = _context.MealTags.Where(mt => mt.MealId == dietId);
            _context.MealTags.RemoveRange(oldTags);
            await _context.SaveChangesAsync();

            // Add new tags
            await AddTagsToDiet(dietId, tags);
        }

        private async Task RemoveTagsFromDiet(int dietId)
        {
            var tags = _context.MealTags.Where(mt => mt.MealId == dietId);
            _context.MealTags.RemoveRange(tags);
            await _context.SaveChangesAsync();
        }
    }
}
