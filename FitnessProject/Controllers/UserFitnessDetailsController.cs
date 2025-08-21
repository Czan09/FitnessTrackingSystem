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
using System.Collections.Generic;

namespace FitnessProject.Controllers
{
    [Authorize(Roles = "Admin,Trainer")]
    public class UserFitnessDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAutoTagService _autoTagService;

        public UserFitnessDetailsController(ApplicationDbContext context, IAutoTagService autoTagService)
        {
            _context = context;
            _autoTagService = autoTagService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.UserFitnessDetails
                        .Include(u => u.User)
                        .ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var user = await _context.UserFitnessDetails.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        public async Task<IActionResult> Create()
        {
            var model = new UserFitnessDetailsViewModel
            {
                GoalOptions = Enum.GetValues(typeof(MemberGoals)).Cast<MemberGoals>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }),
                ExperienceOptions = Enum.GetValues(typeof(ExperinceLevelMember)).Cast<ExperinceLevelMember>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() })
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFitnessDetailsViewModel model)
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

        // GET: UserFitnessDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.UserFitnessDetails.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

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

        // GET: UserFitnessDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var user = await _context.UserFitnessDetails.FirstOrDefaultAsync(m => m.Id == id);
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
                await RemoveTagsFromUser(user.Id);
                _context.UserFitnessDetails.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper methods for managing user tags
        private async Task AddTagsToUser(int userId, List<string> tags)
        {
            foreach (var tagName in tags)
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);

                if (tag == null)
                {
                    tag = new Tags { Name = tagName };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync(); // Save to get tag.Id
                }

                var exists = await _context.UserTags
                    .AnyAsync(ut => ut.userDetailId == userId && ut.TagId == tag.Id);

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
