using FitnessProject.Data;
using FitnessProject.Models.enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessProject.Models
{
    public class UserFitnessDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [MinimumAge(16, ErrorMessage = "Minimum age is 16")]
        public DateOnly DOB { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [Range(100, 300, ErrorMessage = "Height must be between 100 and 300 cm.")]
        public float Height { get; set; }

        [Required]
        [Range(30, 300, ErrorMessage = "Weight must be between 30 and 300 kg.")]
        public float Weight { get; set; }

        [Required]
        public MemberGoals Goal { get; set; }

        [Required]
        public ExperinceLevelMember ExperienceLevel { get; set; }

        [Required]
        public DietType DietType { get; set; }

        public string? FitnessNotes { get; set; }
        public string? MedicalConditions { get; set; }

        [NotMapped]
        public float BMI => Weight / (Height / 100 * Height / 100);

        // For dropdowns and form binding
        [NotMapped]
        public IEnumerable<SelectListItem> GoalOptions { get; set; } = new List<SelectListItem>();

        [NotMapped]
        public IEnumerable<SelectListItem> ExperienceOptions { get; set; } = new List<SelectListItem>();

        [NotMapped]
        public List<int> AssignedPlanIds { get; set; } = new();

        [NotMapped]
        public List<SelectListItem> AvailablePlans { get; set; } = new();

        public ICollection<UserTag> userTags { get; set; } = new List<UserTag>();
        public virtual ApplicationUser? User { get; set; }

        // Custom validation for age
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - DOB.Year;
            if (DOB > today.AddYears(-age)) age--;

            if (age < 16)
            {
                yield return new ValidationResult(
                    "You must be at least 16 years old.",
                    new[] { nameof(DOB) });
            }

            if (age > 90)
            {
                yield return new ValidationResult(
                    "Age cannot be more than 90 years.",
                    new[] { nameof(DOB) });
            }
        }
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
