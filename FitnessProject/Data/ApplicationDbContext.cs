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
        public DbSet<Workout> Workouts { get; set; } //
        public DbSet<Diets> Diets { get; set; } //
        public DbSet<WorkoutPlanUser> WorkoutPlanUsers {get;set;}
        public DbSet<MealPlan> MealPlans { get; set; }
        public DbSet<MealPlanMeals> MealPlanMeals { get; set; }

        public DbSet<Tags>  Tags { get; set; }
        public DbSet<MealTags> MealTags{ get; set; }
        public DbSet<WorkoutTags> WorkoutTags { get; set; }

        public DbSet<UserTag> UserTags { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserFitnessDetails>()
                .HasOne(u => u.User)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MealTags>()
                .HasKey(mt => new { mt.MealId, mt.TagId });
            modelBuilder.Entity<WorkoutTags>()
                .HasKey(wt => new { wt.WorkoutId, wt.TagId });
        }


    }
}
