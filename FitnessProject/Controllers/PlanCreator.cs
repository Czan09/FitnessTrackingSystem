using FitnessProject.Data;
using FitnessProject.Models;
using FitnessProject.Models.enums;
using FitnessProject.RecommendationEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FitnessProject.Controllers
{
    public class PlanCreator : Controller
    {
        private readonly Engine _engine;
        private readonly ApplicationDbContext _context;

        public PlanCreator(ApplicationDbContext context)
        {
            _engine = new Engine();
            _context = context;
        }

        public async Task<IActionResult> FoodPlanGenerate(string userId)
        {
            var userDetails =  await _context.UserFitnessDetails.FirstOrDefaultAsync(u => u.UserId == userId);
            if(userDetails == null){
                return NotFound("User not found");
            }
            var allMeals =  await _context.Diets.ToListAsync();
            var recommendedMeals = _engine.RecommendMeals(userDetails, allMeals);

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
            if (userDetails == null)
                return NotFound("User not found");

            // Get recommended workout types from Engine
            var recommendedTypes = _engine.RecommendWorkouts(userDetails.ExperienceLevel, userDetails.Goal);

            // Query workouts from DB matching those types
            var recommendedWorkouts = await _context.Workouts
                .Where(w => recommendedTypes.Contains(w.Type))
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
            {
                Console.WriteLine(userId);
                Console.WriteLine(planName);
                return BadRequest("Invalid input data.");
            }

            // Create MealPlan
            var mealPlan = new MealPlan
            {
                Name = planName,
                userId = userId
            };

            _context.MealPlans.Add(mealPlan);
            await _context.SaveChangesAsync(); // save to get mealPlan.Id

            // Create MealPlanMeals
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
            {
                return BadRequest("Invalid input data.");
            }

            // Create and save each workout for the user
            foreach (var workoutId in selectedWorkoutIds)
            {
                var entry = new WorkoutPlanUser
                {
                    UserId = userId,
                    WorkoutId = workoutId
                };

                _context.WorkoutPlanUsers.Add(entry);
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
            var workouts = await _context.WorkoutPlanUsers
                .Where(wp => wp.UserId == userId)
                .Include(wp => wp.Workout)
                .ToListAsync();

            return View(workouts);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteMealPlan(int id)
        {
            var plan = await _context.MealPlans.FindAsync(id);
            if (plan == null)
                return NotFound();

            // Also delete associated meals
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

