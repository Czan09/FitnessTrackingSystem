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
    [Authorize(Roles ="Admin,Trainer")]
    public class DietController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DietController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Diet
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10;

            var diets = _context.Diets.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                diets = diets.Where(d => d.MealName.Contains(searchString));
            }

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


        // GET: Diet/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var diet = await _context.Diets.FirstOrDefaultAsync(m => m.Id == id);

            if (diet == null) return NotFound();

            return View(diet);
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
            if (ModelState.IsValid)
            {
                _context.Add(diet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DietTypes"] = new SelectList(Enum.GetValues(typeof(DietType)), diet.Type);
            return View(diet);
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

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Diets.Any(e => e.Id == id))
                        return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DietTypes"] = new SelectList(Enum.GetValues(typeof(DietType)), diet.Type);
            return View(diet);
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
                _context.Diets.Remove(diet);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
