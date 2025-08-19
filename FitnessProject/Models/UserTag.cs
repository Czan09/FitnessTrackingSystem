using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessProject.Models
{
    public class UserTag
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int userDetailId { get; set; }
        [Required]
        public int TagId { get; set; }
        // Navigation properties
        [ForeignKey("userDetailId")]
        public UserFitnessDetails? UserFitnessDetails{ get; set; }

        [ForeignKey("TagId")]
        public Tags? Tag { get; set; }
    }
}
