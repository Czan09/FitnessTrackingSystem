using FitnessProject.Data;
using FitnessProject.Models;
using FitnessProject.Models.enums;
using FitnessProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessProject.Controllers
{
    [Authorize(Roles = "Admin,Trainer")]
    public class UserFitnessDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserFitnessDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserFitnessDetails
        public async Task<IActionResult> Index()
        {
            var users = await _context.UserFitnessDetails
                        .Include(u => u.User) 
                        .Include(u => u.AssignedPlans)
                        .ToListAsync();
            return View(users);
        }

        // GET: UserFitnessDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.UserFitnessDetails
                .Include(u => u.AssignedPlans)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // GET: UserFitnessDetails/Create
        public async Task<IActionResult> Create()
        {
            var plans = await _context.WorkoutPlans.ToListAsync();

            var model = new UserFitnessDetailsViewModel
            {
                AvailablePlans = plans
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Title })
                    .ToList(),
                GoalOptions = Enum.GetValues(typeof(MemberGoals)).Cast<MemberGoals>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }),
                ExperienceOptions = Enum.GetValues(typeof(ExperinceLevelMember)).Cast<ExperinceLevelMember>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
            };

            return View(model);
        }

        // POST: UserFitnessDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFitnessDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new UserFitnessDetails
                {
                    UserId = model.UserId,
                    DOB = model.DOB,
                    Gender = model.Gender,
                    Height = model.Height,
                    Weight = model.Weight,
                    Goal = model.Goal,
                    ExperienceLevel = model.ExperienceLevel
                };


                var selectedPlans = await _context.WorkoutPlans
                    .Where(p => model.AssignedPlanIds.Contains(p.Id))
                    .ToListAsync();

                user.AssignedPlans = selectedPlans;

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Reload dropdowns
            model.AvailablePlans = await _context.WorkoutPlans
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Title })
                .ToListAsync();
            model.GoalOptions = Enum.GetValues(typeof(MemberGoals)).Cast<MemberGoals>()
                .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() });
            model.ExperienceOptions = Enum.GetValues(typeof(ExperinceLevelMember)).Cast<ExperinceLevelMember>()
                .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() });

            return View(model);
        }

        // GET: UserFitnessDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.UserFitnessDetails
                .Include(u => u.AssignedPlans)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var plans = await _context.WorkoutPlans.ToListAsync();

            var model = new UserFitnessDetailsViewModel
            {
                Id = user.Id,
                UserId = user.UserId,
                DOB = user.DOB,
                Gender = user.Gender,
                Height = user.Height,
                Weight = user.Weight,
                Goal = user.Goal,
                ExperienceLevel = user.ExperienceLevel,
                AssignedPlanIds = user.AssignedPlans.Select(p => p.Id).ToList(),
                AvailablePlans = plans
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Title })
                    .ToList(),
                GoalOptions = Enum.GetValues(typeof(MemberGoals)).Cast<MemberGoals>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }),
                ExperienceOptions = Enum.GetValues(typeof(ExperinceLevelMember)).Cast<ExperinceLevelMember>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
            };

            return View(model);
        }

        // POST: UserFitnessDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserFitnessDetailsViewModel model)
        {
            Console.WriteLine("Edit Reached Here");
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                model.AvailablePlans = await _context.WorkoutPlans
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Title })
                    .ToListAsync();
                model.GoalOptions = Enum.GetValues(typeof(MemberGoals)).Cast<MemberGoals>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() });
                model.ExperienceOptions = Enum.GetValues(typeof(ExperinceLevelMember)).Cast<ExperinceLevelMember>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() });

                return View(model);
            }

            var user = await _context.UserFitnessDetails
                .Include(u => u.AssignedPlans)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            // Update fields
            user.DOB = model.DOB;
            user.Gender = model.Gender;
            user.Height = model.Height;
            user.Weight = model.Weight;
            user.Goal = model.Goal;
            user.ExperienceLevel = model.ExperienceLevel;
            Console.WriteLine("UserDataEdit:::"+user);

            // Clear and update assigned plans
            user.AssignedPlans.Clear();
            if (model.AssignedPlanIds != null)
            {
                var selectedPlans = await _context.WorkoutPlans
                    .Where(p => model.AssignedPlanIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var plan in selectedPlans)
                {
                    user.AssignedPlans.Add(plan);
                }
            }

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User fitness details updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UserFitnessDetails.Any(e => e.Id == user.Id))
                    return NotFound();

                throw;
            }
        }

        private bool UserFitnessDetailsExists(int id)
        {
            return _context.UserFitnessDetails.Any(e => e.Id == id);
        }

        // GET: UserFitnessDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.UserFitnessDetails
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // POST: UserFitnessDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.UserFitnessDetails.FindAsync(id);
            if (user != null)
            {
                _context.UserFitnessDetails.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
