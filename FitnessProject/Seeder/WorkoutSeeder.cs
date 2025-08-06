using FitnessProject.Data;
using FitnessProject.Models;
using FitnessProject.Models.enums;
using Microsoft.EntityFrameworkCore;

namespace FitnessProject.Seeder
{
    public class WorkoutPlanSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly Random _random = new();

        private readonly int fixedTrainerId = 1;

        public WorkoutPlanSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedWorkoutPlansAsync(int numberOfPlans = 20)
        {
            if (_context.WorkoutPlans.Any())
                return; // Already seeded

            var allWorkouts = await _context.Workouts.ToListAsync();

            var goals = Enum.GetValues(typeof(MemberGoals)).Cast<MemberGoals>().ToList();

            for (int i = 1; i <= numberOfPlans; i++)
            {
                var goal = goals[_random.Next(goals.Count)];

                var plan = new WorkoutPlan
                {
                    Title = $"Workout Plan #{i} for {goal}",
                    Description = $"Auto-generated workout plan focused on {goal}.",
                    TrainerId = 1,
                    Goal = goal
                };

                // Select 3-7 unique workouts randomly
                var selectedWorkouts = allWorkouts.OrderBy(x => Guid.NewGuid()).Take(_random.Next(3, 8)).ToList();

                foreach (var workout in selectedWorkouts)
                {
                    plan.Workouts.Add(workout);
                }

                _context.WorkoutPlans.Add(plan);
            }

            await _context.SaveChangesAsync();
        }
    }
}