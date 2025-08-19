using FitnessProject.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace FitnessProject.Services
{
    public class BulkAddTag
    {
        private ApplicationDbContext _context;
        private HttpClient _httpClient = new HttpClient();
        public BulkAddTag(ApplicationDbContext context)
        {
            _context = context;
            _httpClient.BaseAddress = new Uri("http://127.0.0.1:8000");
        }

        public async Task AddTagsToDietsAsync()
        {
            var diets = await _context.Diets.ToListAsync();

            foreach (var diet in diets)
            {
                AutoTagService tagService = new AutoTagService(_httpClient);

                // Fetch the tags from API
                var tags = await tagService.GetMealTagsAsync(diet.Id);

                if (tags != null && tags.Any())
                {
                    TagService ts = new TagService(_context);
                    await ts.AddTagsToMealAsync(diet.Id, tags);
                }
            }

            await _context.SaveChangesAsync();
        }
        public async Task AddTagsToWorkoutsAsync()
        {
            var workouts = await _context.Workouts.ToListAsync();

            foreach (var workout in workouts)
            {
                AutoTagService tagService = new AutoTagService(_httpClient);

                // Fetch the tags from API
                var tags = await tagService.GetWorkoutTagsAsync(workout.Id);

                if (tags != null && tags.Any())
                {
                    TagService ts = new TagService(_context);
                    await ts.AddTagsToWorkoutAsync(workout.Id, tags);
                }
            }

            await _context.SaveChangesAsync();
        }
        public async Task AddTagsToUserDetailsAsync()
        {
            var users= await _context.UserFitnessDetails.ToListAsync();

            foreach (var user in users)
            {
                AutoTagService tagService = new AutoTagService(_httpClient);

                // Fetch the tags from API
                var tags = await tagService.GetUserFitnessTags(user.Id);

                if (tags != null && tags.Any())
                {
                    TagService ts = new TagService(_context);
                    await ts.AddTagsToUserFitnessAsync(user.Id, tags);
                }
            }

            await _context.SaveChangesAsync();
        }

    }
}