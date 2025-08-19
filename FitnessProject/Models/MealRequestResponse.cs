namespace FitnessProject.Models
{
    public class MealRequestResponse
    {
        public Diets diet {  get; set; } = new Diets();
        public List<String> Tags { get; set; } = new List<String>();
    }
    public class ApiResponse
    {
        public MealRequestResponse data { get; set; } = new MealRequestResponse();
    }
}
