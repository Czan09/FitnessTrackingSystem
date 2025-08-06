using FitnessProject.Data;
using FitnessProject.Models;
using FitnessProject.Models.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessProject.Controllers
{
    [Authorize]
    public class UserDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserDetailsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /UserDetails
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);
            var userProfile = await _context.UserFitnessDetails
                .Include(u => u.AssignedPlans)
                .FirstOrDefaultAsync(u => u.UserId == currentUserId);

            if (userProfile == null)
                return RedirectToAction(nameof(CreateProfile));

            return View(userProfile);
        }

        // GET: /UserDetails/CreateProfile
        public IActionResult CreateProfile()
        {
            var model = new UserFitnessDetails
            {
                DOB = DateOnly.FromDateTime(DateTime.Today.AddDays(-16)),
                UserId = _userManager.GetUserId(User)
            };

            return View(model);
        }


        // POST: /UserDetails/CreateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfile(UserFitnessDetails model)
        {
            //Console.WriteLine("Reached CreateProfile POST action");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("[Debug] ModelState is invalid:");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"[ModelState Error] {state.Key}: {error.ErrorMessage}");
                    }
                }
                return View(model);
            }

            //Console.WriteLine("[Debug] Incoming model values:");
            //Console.WriteLine($"DOB: {model.DOB}");
            //Console.WriteLine($"Gender: {model.Gender}");
            //Console.WriteLine($"Height: {model.Height}");
            //Console.WriteLine($"Weight: {model.Weight}");
            //Console.WriteLine($"Goal: {model.Goal}");
            //Console.WriteLine($"ExperienceLevel: {model.ExperienceLevel}");

            model.UserId = _userManager.GetUserId(User);
            //Console.WriteLine($"[Debug] UserId from _userManager: {model.UserId}");

            _context.UserFitnessDetails.Add(model);
            await _context.SaveChangesAsync();

            //Console.WriteLine("[Debug] Successfully saved to database.");
            return RedirectToAction(nameof(Index));
        }


        // GET: /UserDetails/EditProfile
        public async Task<IActionResult> EditProfile()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.UserFitnessDetails
                .Include(u => u.AssignedPlans)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return RedirectToAction(nameof(CreateProfile));

            return View(user);
        }

        // POST: /UserDetails/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserFitnessDetails model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = _userManager.GetUserId(User);
            var user = await _context.UserFitnessDetails.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound();

            user.DOB = model.DOB;
            user.Gender = model.Gender;
            user.Height = model.Height;
            user.Weight = model.Weight;
            user.Goal = model.Goal;
            user.ExperienceLevel = model.ExperienceLevel;
            user.DietType = model.DietType;
            user.FitnessNotes = model.FitnessNotes;
            user.MedicalConditions = model.MedicalConditions;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /UserDetails/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /UserDetails/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "New passwords do not match.");
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
