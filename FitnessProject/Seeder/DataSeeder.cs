using FitnessProject.Data;
using FitnessProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessProject.Seeder
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly Random _random = new();

        public DataSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedMealPlansAsync()
        {
            if (_context.MealPlans.Any() || _context.MealPlanDiets.Any())
                return;

            var allDiets = await _context.Diets.ToListAsync();

            string[] days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
            string[] mealTimes = ["Breakfast", "Lunch", "Dinner", "Snack"];

            for (int i = 1; i <= 30; i++) // Create 30 meal plans
            {
                var day = days[i % days.Length];

                var plan = new MealPlan
                {
                    PlanName = $"Plan for {day} #{i}",
                    Description = $"Automatically generated meal plan for {day}",
                    Day = day
                };

                int totalCalories = 0;

                var selectedMeals = allDiets.OrderBy(x => Guid.NewGuid()).Take(_random.Next(3, 6)).ToList();

                foreach (var diet in selectedMeals)
                {
                    string mealTime = mealTimes[_random.Next(mealTimes.Length)];

                    plan.Meals.Add(new MealPlanDiet
                    {
                        DietId = diet.Id,
                        MealTime = mealTime
                    });

                    totalCalories += diet.Calories;
                }

                plan.TotalCalories = totalCalories;

                _context.MealPlans.Add(plan);
            }

            await _context.SaveChangesAsync();
        }
    }
}
