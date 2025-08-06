using Microsoft.AspNetCore.Identity;

namespace FitnessProject.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string OTPCode { get; set; }
        public DateTime? OTPExpiry { get; set; }
    }
}
