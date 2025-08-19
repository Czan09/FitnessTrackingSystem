using FitnessProject.Data;
using FitnessProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessProject.Services
{
    public class TagService
    {
        private readonly ApplicationDbContext _context;

        public TagService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddTagsToMealAsync(int mealId, List<string> tags)
        {
            var meal = await _context.Diets
                .Include(m => m.MealTags)
                .ThenInclude(mt => mt.Tag)
                .FirstOrDefaultAsync(m => m.Id == mealId);

            if (meal == null) return;

            foreach (var tagName in tags)
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName)
                          ?? new Tags { Name = tagName };

                if (!_context.Tags.Any(t => t.Name == tagName))
                    _context.Tags.Add(tag);

                if (!meal.MealTags.Any(mt => mt.Tag.Name == tagName))
                    meal.MealTags.Add(new MealTags { Meal = meal, Tag = tag });
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddTagsToWorkoutAsync(int workoutId, List<string> tags)
        {
            var workout = await _context.Workouts
                .Include(w => w.WorkoutTags)
                .ThenInclude(wt => wt.Tag)
                .FirstOrDefaultAsync(w => w.Id == workoutId);

            if (workout == null) return;

            foreach (var tagName in tags)
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName)
                          ?? new Tags { Name = tagName };

                if (!_context.Tags.Any(t => t.Name == tagName))
                    _context.Tags.Add(tag);

                if (!workout.WorkoutTags.Any(wt => wt.Tag.Name == tagName))
                    workout.WorkoutTags.Add(new WorkoutTags { Workout = workout, Tag = tag });
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddTagsToUserFitnessAsync(int Id, List<string> tags)
        {
            var userFitness = await _context.UserFitnessDetails
                .Include(w => w.userTags)
                .ThenInclude(wt => wt.Tag)
                .FirstOrDefaultAsync(w => w.Id == Id);

            if (userFitness == null) return;

            foreach (var tagName in tags)
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName)
                          ?? new Tags { Name = tagName };

                if (!_context.Tags.Any(t => t.Name == tagName))
                    _context.Tags.Add(tag);

                if (!userFitness.userTags.Any(wt => wt.Tag.Name == tagName))
                    userFitness.userTags.Add(new UserTag{ userDetailId= Id, Tag = tag });
            }

            await _context.SaveChangesAsync();
        }
    }
}
