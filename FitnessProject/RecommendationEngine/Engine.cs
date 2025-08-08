using FitnessProject.Models;
using FitnessProject.Models.enums;

namespace FitnessProject.RecommendationEngine
{
    public class Engine
    {
        public List<WorkoutType> RecommendWorkouts(ExperinceLevelMember experience, MemberGoals goal)
        {
            var workouts = new List<WorkoutType>();

            if (goal == MemberGoals.WeightLoss)
            {
                if (experience == ExperinceLevelMember.None)
                    workouts.AddRange(new[] { WorkoutType.Cardio, WorkoutType.FunctionalFitness });
                else if (experience == ExperinceLevelMember.Some)
                    workouts.AddRange(new[] { WorkoutType.Cardio, WorkoutType.StrengthTraining });
                else if (experience == ExperinceLevelMember.Expert)
                    workouts.AddRange(new[] { WorkoutType.Cardio, WorkoutType.StrengthTraining, WorkoutType.SportsSpecific });
            }
            else if (goal == MemberGoals.FitBody)
            {
                if (experience == ExperinceLevelMember.None)
                    workouts.AddRange(new[] { WorkoutType.FunctionalFitness, WorkoutType.Mobility });
                else if (experience == ExperinceLevelMember.Some)
                    workouts.AddRange(new[] { WorkoutType.StrengthTraining, WorkoutType.Flexibility });
                else if (experience == ExperinceLevelMember.Expert)
                    workouts.AddRange(new[] { WorkoutType.StrengthTraining, WorkoutType.SportsSpecific });
            }
            else if (goal == MemberGoals.StrongerBody)
            {
                if (experience == ExperinceLevelMember.None)
                    workouts.AddRange(new[] { WorkoutType.FunctionalFitness, WorkoutType.Mobility });
                else if (experience == ExperinceLevelMember.Some)
                    workouts.AddRange(new[] { WorkoutType.StrengthTraining, WorkoutType.BalanceAndStability });
                else if (experience == ExperinceLevelMember.Expert)
                    workouts.AddRange(new[] { WorkoutType.StrengthTraining, WorkoutType.SportsSpecific });
            }
            else if (goal == MemberGoals.SelfEsteem)
            {
                if (experience == ExperinceLevelMember.None)
                    workouts.AddRange(new[] { WorkoutType.Cardio, WorkoutType.Flexibility });
                else if (experience == ExperinceLevelMember.Some)
                    workouts.AddRange(new[] { WorkoutType.Cardio, WorkoutType.FunctionalFitness });
                else if (experience == ExperinceLevelMember.Expert)
                    workouts.AddRange(new[] { WorkoutType.StrengthTraining, WorkoutType.Flexibility });
            }
            else if (goal == MemberGoals.Disipline)
            {
                if (experience == ExperinceLevelMember.None)
                    workouts.AddRange(new[] { WorkoutType.FunctionalFitness, WorkoutType.Mobility });
                else if (experience == ExperinceLevelMember.Some)
                    workouts.AddRange(new[] { WorkoutType.StrengthTraining, WorkoutType.Flexibility });
                else if (experience == ExperinceLevelMember.Expert)
                    workouts.AddRange(new[] { WorkoutType.StrengthTraining, WorkoutType.SportsSpecific });
            }

            return workouts.Distinct().ToList();
        }

        public List<Diets> RecommendMeals(UserFitnessDetails user, List<Diets> allMeals)
        {
            var filteredMeals = allMeals
                .Where(m => m.Type == user.DietType)
                .ToList();

            float bmi = user.BMI;
            int minCalories = 0, maxCalories = 0;

            // Determine calorie range based on goal and BMI
            switch (user.Goal)
            {
                case MemberGoals.WeightLoss:
                    minCalories = 250; maxCalories = 400;
                    break;

                case MemberGoals.FitBody:
                    if (bmi < 18.5) { minCalories = 500; maxCalories = 600; }
                    else { minCalories = 400; maxCalories = 500; }
                    break;

                case MemberGoals.StrongerBody:
                    minCalories = 500; maxCalories = 700;
                    break;

                case MemberGoals.SelfEsteem:
                case MemberGoals.Disipline:
                    minCalories = 350; maxCalories = 500;
                    break;
            }

            // Filter by calories
            var recommended = filteredMeals
                .Where(m => m.Calories >= minCalories && m.Calories <= maxCalories)
                .OrderBy(m => m.Calories)
                .ToList();

            return recommended;
        }
    }
}
