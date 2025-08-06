using FitnessProject.Models;
using FitnessProject.Models.enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FitnessProject.Models
{
    public class UserFitnessDetailsViewModel
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

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
    }
}