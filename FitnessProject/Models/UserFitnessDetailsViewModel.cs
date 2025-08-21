using FitnessProject.Data;
using FitnessProject.Models;
using FitnessProject.Models.enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessProject.Models
{
    public class UserFitnessDetailsViewModel
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1940-01-01", "2009-12-31",
         ErrorMessage = "Date of Birth must be between 1940 and 2009.")]
        public DateOnly DOB { get; set; }

        public Gender Gender { get; set; }

        public float Height { get; set; }

        public float Weight { get; set; }

        [Required]
        public MemberGoals Goal { get; set; }

        [Required]
        public ExperinceLevelMember ExperienceLevel { get; set; }

        public List<int> AssignedPlanIds { get; set; } = new();

        public List<SelectListItem> AvailablePlans { get; set; } = new();

        public IEnumerable<SelectListItem> GoalOptions { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> ExperienceOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - DOB.Year;
            if (DOB > today.AddYears(-age)) age--; // adjust if not birthday yet

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
            public string? FitnessNotes { get; set; }
        public string? MedicalConditions { get; set; }
        [NotMapped]
        public float BMI => Weight / (Height / 100 * Height / 100);

        public ICollection<UserTag> userTags { get; set; } = new List<UserTag>();

        public virtual ApplicationUser? User { get; set; }
    }
    }
