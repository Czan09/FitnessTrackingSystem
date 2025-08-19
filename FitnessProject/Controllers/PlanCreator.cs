using FitnessProject.Data;
using FitnessProject.Models;
using FitnessProject.Models.enums;
using FitnessProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FitnessProject.Controllers
{
    public class PlanCreator : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlanCreator(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper method to call Python microservice
        private async Task<dynamic> GetPythonRecommendationsAsync(int userId)
        {
            string pythonEndpoint = $"http://127.0.0.1:8000/autoTag/recommendation/{userId}";
            using var client = new HttpClient();
            var response = await client.GetAsync(pythonEndpoint);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<dynamic>(content);
        }

        public async Task<IActionResult> FoodPlanGenerate(string userId)
        {
            var userDetails = await _context.UserFitnessDetails.FirstOrDefaultAsync(u => u.UserId == userId);
            if (userDetails == null) return NotFound("User not found");

            var result = await GetPythonRecommendationsAsync(userDetails.Id);
            if (result == null) return BadRequest("Failed to get recommendations");

            List<int> recommendedMealIds = new List<int>();
            foreach (var meal in result.data.recommended_meals)
                recommendedMealIds.Add((int)meal.MealId);

            var recommendedMeals = await _context.Diets
                .Where(d => recommendedMealIds.Contains(d.Id))
                .ToListAsync();

            var vm = new PlanResultViewModel
            {
                RecommendedMeals = recommendedMeals,
                UserId = userId,
            };

            return View(vm);
        }

        public async Task<IActionResult> WorkoutPlanGenerate(string userId)
        {
            var userDetails = await _context.UserFitnessDetails.FirstOrDefaultAsync(u => u.UserId == userId);
            if (userDetails == null) return NotFound("User not found");

            var result = await GetPythonRecommendationsAsync(userDetails.Id);
            if (result == null) return BadRequest("Failed to get recommendations");

            List<int> recommendedWorkoutIds = new List<int>();
            foreach (var workout in result.data.recommended_workouts)
                recommendedWorkoutIds.Add((int)workout.WorkoutId);

            var recommendedWorkouts = await _context.Workouts
                .Where(w => recommendedWorkoutIds.Contains(w.Id))
                .ToListAsync();

            var vm = new PlanResultViewModel
            {
                RecommendedWorkouts = recommendedWorkouts,
                UserId = userId,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> SaveFoodPlan(string userId, string planName, List<int> selectedMealIds)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(planName) || selectedMealIds == null || selectedMealIds.Count == 0)
                return BadRequest("Invalid input data.");

            var mealPlan = new MealPlan
            {
                Name = planName,
                userId = userId
            };

            _context.MealPlans.Add(mealPlan);
            await _context.SaveChangesAsync();

            foreach (var mealId in selectedMealIds)
            {
                _context.MealPlanMeals.Add(new MealPlanMeals
                {
                    mealPlanId = mealPlan.Id,
                    foodId = mealId
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "UserDetails");
        }

        [HttpPost]
        public async Task<IActionResult> SaveWorkoutPlan(string userId, string planName, List<int> selectedWorkoutIds)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(planName) || selectedWorkoutIds == null || selectedWorkoutIds.Count == 0)
                return BadRequest("Invalid input data.");

            foreach (var workoutId in selectedWorkoutIds)
            {
                _context.WorkoutPlanUsers.Add(new WorkoutPlanUser
                {
                    Name = planName,
                    UserId = userId,
                    WorkoutId = workoutId
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "UserDetails");
        }

        public async Task<IActionResult> MealPlans(string userId)
        {
            var plans = await _context.MealPlans
                .Where(p => p.userId == userId)
                .Include(p => p.User)
                .ToListAsync();

            return View(plans);
        }

        public async Task<IActionResult> MealPlanDetails(int id)
        {
            var meals = await _context.MealPlanMeals
                .Where(mp => mp.mealPlanId == id)
                .Include(mp => mp.Meal)
                .ToListAsync();

            ViewBag.PlanName = await _context.MealPlans
                .Where(p => p.Id == id)
                .Select(p => p.Name)
                .FirstOrDefaultAsync();

            return View(meals);
        }

        public async Task<IActionResult> Workouts(string userId)
        {
            var plans = await _context.WorkoutPlanUsers
                .Where(wp => wp.UserId == userId)
                .Include(wp => wp.Workout)
                .GroupBy(wp => wp.Name)
                .Select(g => new WorkoutPlanViewModel
                {
                    PlanName = g.Key,
                    Workouts = g.Select(x => x.Workout).ToList()
                })
                .ToListAsync();

            ViewBag.UserId = userId;
            return View(plans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePlan(string planName, string userId)
        {
            if (string.IsNullOrEmpty(planName) || string.IsNullOrEmpty(userId))
                return BadRequest("Invalid data.");

            var workouts = await _context.WorkoutPlanUsers
                .Where(wp => wp.UserId == userId && wp.Name == planName)
                .ToListAsync();

            if (workouts.Any())
            {
                _context.WorkoutPlanUsers.RemoveRange(workouts);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Workouts), new { userId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMealPlan(int id)
        {
            var plan = await _context.MealPlans.FindAsync(id);
            if (plan == null) return NotFound();

            var planMeals = await _context.MealPlanMeals
                .Where(mp => mp.mealPlanId == id)
                .ToListAsync();

            _context.MealPlanMeals.RemoveRange(planMeals);
            _context.MealPlans.Remove(plan);

            await _context.SaveChangesAsync();
            return RedirectToAction("MealPlans", new { userId = plan.userId });
        }
    }
}
