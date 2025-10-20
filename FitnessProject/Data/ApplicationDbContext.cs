using FitnessProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserFitnessDetails> UserFitnessDetails { get; set; }
        public DbSet<TrainerDetails> TrainerDetails { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Diets> Diets { get; set; }
        public DbSet<WorkoutPlanUser> WorkoutPlanUsers { get; set; }
        public DbSet<MealPlan> MealPlans { get; set; }
        public DbSet<MealPlanMeals> MealPlanMeals { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<MealTags> MealTags { get; set; }
        public DbSet<WorkoutTags> WorkoutTags { get; set; }

        public DbSet<WorkoutPlanTrainer> WorkoutPlanTrainers { get; set; }
        public DbSet<WorkoutPlanTrainerWorkout> WorkoutPlanTrainerWorkouts { get; set; } // new join table

        public DbSet<UserTag> UserTags { get; set; }
        public DbSet<MealPlanTrainer> MealPlanTrainers { get; set; }
        public DbSet<MealPlanTrainerMeal> MealPlanTrainerMeals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserFitnessDetails
            modelBuilder.Entity<UserFitnessDetails>()
                .HasOne(u => u.User)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // MealTags & WorkoutTags keys
            modelBuilder.Entity<MealTags>()
                .HasKey(mt => new { mt.MealId, mt.TagId });

            modelBuilder.Entity<WorkoutTags>()
                .HasKey(wt => new { wt.WorkoutId, wt.TagId });

            // TrainerDetails -> ApplicationUser (nullable, many-to-one)
            modelBuilder.Entity<TrainerDetails>()
                .HasOne(t => t.ApplicationUser)
                .WithMany()
                .HasForeignKey(t => t.ApplicationUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // WorkoutPlanTrainer -> TrainerDetails
            modelBuilder.Entity<WorkoutPlanTrainer>()
                .HasOne(w => w.Trainer)
                .WithMany()
                .HasForeignKey(w => w.TrainerId)
                .OnDelete(DeleteBehavior.NoAction);

            // WorkoutPlanTrainer -> ApplicationUser (User)
            modelBuilder.Entity<WorkoutPlanTrainer>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // WorkoutPlanTrainer <-> Workout (many-to-many)
            modelBuilder.Entity<WorkoutPlanTrainerWorkout>()
                .HasKey(wptw => new { wptw.WorkoutPlanTrainerId, wptw.WorkoutId });

            modelBuilder.Entity<WorkoutPlanTrainerWorkout>()
                .HasOne(wptw => wptw.WorkoutPlanTrainer)
                .WithMany(wpt => wpt.WorkoutPlanTrainerWorkouts)
                .HasForeignKey(wptw => wptw.WorkoutPlanTrainerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkoutPlanTrainerWorkout>()
                .HasOne(wptw => wptw.Workout)
                .WithMany()
                .HasForeignKey(wptw => wptw.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<MealPlanTrainerMeal>()
                  .HasKey(mptm => new { mptm.MealPlanTrainerId, mptm.MealId });

            modelBuilder.Entity<MealPlanTrainerMeal>()
                .HasOne(mptm => mptm.MealPlanTrainer)
                .WithMany(mpt => mpt.MealPlanTrainerMeals)
                .HasForeignKey(mptm => mptm.MealPlanTrainerId);

            modelBuilder.Entity<MealPlanTrainerMeal>()
                .HasOne(mptm => mptm.Meal)
                .WithMany()
                .HasForeignKey(mptm => mptm.MealId);

        }
    }
}
