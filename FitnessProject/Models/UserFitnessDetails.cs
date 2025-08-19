using FitnessProject.Data;
using FitnessProject.Models.enums;
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
        [MinimumAge(16,ErrorMessage ="Minimum age is 16")]
        public DateOnly DOB { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        [Range(100, 300, ErrorMessage = "Height must be at least 100 cm.")]
        public float Height { get; set; }
        [Required]
        [Range(30, 300, ErrorMessage = "Weight must be at least 30 kg.")]
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

        public ICollection<UserTag> userTags { get; set; } = new List<UserTag>();

        public virtual ApplicationUser? User { get; set; }


        //public static implicit operator UserFitnessDetails(UserFitnessDetails v)
        //{
        //    throw new NotImplementedException();
        //}
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
