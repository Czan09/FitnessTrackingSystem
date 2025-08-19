using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FitnessProject.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Tags
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
