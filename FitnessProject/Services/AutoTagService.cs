using FitnessProject.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

public interface IAutoTagService
{
    Task<List<string>> GetMealTagsAsync(int mealId);
    Task<List<string>> GetUserFitnessTags(int userId);
    Task<List<string>> GetWorkoutTagsAsync(int workoutId);
}

public class AutoTagService : IAutoTagService
{
    private readonly HttpClient _httpClient;

    public AutoTagService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<string>> GetMealTagsAsync(int mealId)
    {
        var response = await _httpClient.GetAsync($"/autoTag/meals/{mealId}");
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();

        // Deserialize into the correct model
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return apiResponse?.data?.Tags ?? new List<string>();
    }

    public async Task<List<string>> GetWorkoutTagsAsync(int workoutId)
    {
        var response = await _httpClient.GetAsync($"/autoTag/workouts/{workoutId}");
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();

        // Deserialize into the correct model
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return apiResponse?.data?.Tags ?? new List<string>();
    }

    public async Task<List<string>> GetUserFitnessTags(int user_id)
    {
        var response = await _httpClient.GetAsync($"/autoTag/user/{user_id}");
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();

        // Deserialize into the correct model
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return apiResponse?.data?.Tags ?? new List<string>();
    }
}
