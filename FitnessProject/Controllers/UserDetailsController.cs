using FitnessProject.Data;
using FitnessProject.Models;
using FitnessProject.Models.enums;
using FitnessProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FitnessProject.Controllers
{
    [Authorize]
    public class UserDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAutoTagService _autoTagService;

        public UserDetailsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IAutoTagService autoTagService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _autoTagService = autoTagService;
        }

        // ---------- PROFILE INDEX ----------
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "No Role Assigned";

            var userProfile = await _context.UserFitnessDetails
                .Include(u => u.userTags)
                .ThenInclude(ut => ut.Tag)
                .FirstOrDefaultAsync(u => u.UserId == currentUserId);

            if (userProfile == null && currentUserRole == "Member")
                return RedirectToAction(nameof(CreateProfile));

            return View(userProfile);
        }

        // ---------- CREATE PROFILE ----------
        public IActionResult CreateProfile()
        {
            var model = new UserFitnessDetails
            {
                DOB = DateOnly.FromDateTime(DateTime.Today.AddYears(-16)),
                UserId = _userManager.GetUserId(User),
                GoalOptions = Enum.GetValues(typeof(MemberGoals)).Cast<MemberGoals>()
                                .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }),
                ExperienceOptions = Enum.GetValues(typeof(ExperinceLevelMember)).Cast<ExperinceLevelMember>()
                                .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfile(UserFitnessDetails model)
        {
            if (!ModelState.IsValid)
            {
                model.GoalOptions = Enum.GetValues(typeof(MemberGoals)).Cast<MemberGoals>()
                                .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() });
                model.ExperienceOptions = Enum.GetValues(typeof(ExperinceLevelMember)).Cast<ExperinceLevelMember>()
                                .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() });
                return View(model);
            }

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

            _context.Add(user);
            await _context.SaveChangesAsync();

            var tags = await _autoTagService.GetUserFitnessTags(user.Id);
            await AddTagsToUser(user.Id, tags);

            return RedirectToAction(nameof(Index));
        }

        // ---------- EDIT PROFILE ----------
        public async Task<IActionResult> EditProfile()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.UserFitnessDetails.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return RedirectToAction(nameof(CreateProfile));

            var model = new UserFitnessDetails
            {
                Id = user.Id,
                UserId = user.UserId,
                DOB = user.DOB,
                Gender = user.Gender,
                Height = user.Height,
                Weight = user.Weight,
                Goal = user.Goal,
                ExperienceLevel = user.ExperienceLevel,
                GoalOptions = Enum.GetValues(typeof(MemberGoals)).Cast<MemberGoals>()
                                .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }),
                ExperienceOptions = Enum.GetValues(typeof(ExperinceLevelMember)).Cast<ExperinceLevelMember>()
                                .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(int id, UserFitnessDetails model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                model.GoalOptions = Enum.GetValues(typeof(MemberGoals)).Cast<MemberGoals>()
                                .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() });
                model.ExperienceOptions = Enum.GetValues(typeof(ExperinceLevelMember)).Cast<ExperinceLevelMember>()
                                .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() });
                return View(model);
            }

            var user = await _context.UserFitnessDetails.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            user.DOB = model.DOB;
            user.Gender = model.Gender;
            user.Height = model.Height;
            user.Weight = model.Weight;
            user.Goal = model.Goal;
            user.ExperienceLevel = model.ExperienceLevel;

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();

                // Update tags
                var tags = await _autoTagService.GetUserFitnessTags(user.Id);
                await UpdateTagsForUser(user.Id, tags);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UserFitnessDetails.Any(e => e.Id == user.Id))
                    return NotFound();
                else throw;
            }

            TempData["SuccessMessage"] = "User fitness details updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ---------- DELETE PROFILE ----------
        public async Task<IActionResult> DeleteProfile()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.UserFitnessDetails.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("DeleteProfile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProfileConfirmed()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.UserFitnessDetails.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                await RemoveTagsFromUser(user.Id);
                _context.UserFitnessDetails.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------- CHANGE PASSWORD ----------
        public IActionResult ChangePassword() => View();

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
            if (user == null) return RedirectToAction("Login", "Account");

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------- TAG MANAGEMENT ----------
        private async Task AddTagsToUser(int userId, List<string> tags)
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

                var exists = await _context.UserTags.AnyAsync(ut => ut.userDetailId == userId && ut.TagId == tag.Id);
                if (!exists)
                {
                    _context.UserTags.Add(new UserTag { userDetailId = userId, TagId = tag.Id });
                }
            }
            await _context.SaveChangesAsync();
        }

        private async Task UpdateTagsForUser(int userId, List<string> tags)
        {
            var oldTags = _context.UserTags.Where(ut => ut.userDetailId == userId);
            _context.UserTags.RemoveRange(oldTags);
            await _context.SaveChangesAsync();

            await AddTagsToUser(userId, tags);
        }

        private async Task RemoveTagsFromUser(int userId)
        {
            var userTags = _context.UserTags.Where(ut => ut.userDetailId == userId);
            _context.UserTags.RemoveRange(userTags);
            await _context.SaveChangesAsync();
        }
    }
}
