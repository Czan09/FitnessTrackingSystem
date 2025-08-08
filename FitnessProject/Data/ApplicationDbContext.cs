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

        public DbSet<UserFitnessDetails> UserFitnessDetails { get; set; } //
        public DbSet<TrainerDetails> TrainerDetails { get; set; } //
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }//
        public DbSet<Workout> Workouts { get; set; } //
        public DbSet<Diets> Diets { get; set; } //
        public DbSet<WorkoutPlanUser> WorkoutPlanUsers {get;set;}
        public DbSet<MealPlan> MealPlans { get; set; }
        public DbSet<MealPlanMeals> MealPlanMeals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserFitnessDetails>()
                .HasOne(u => u.User)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);
           

        }


    }
}
