using FitnessProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace FitnessProject.uitls
{
    public class PasswordManager
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public PasswordManager(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // STEP 1: Send OTP
        public async Task<bool> SendOtpAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var otp = new Random().Next(100000, 999999).ToString(); // 6-digit OTP
            user.OTPCode = otp;
            user.OTPExpiry = DateTime.UtcNow.AddMinutes(5);

            await _userManager.UpdateAsync(user);

            Console.WriteLine($"[DEBUG] OTP generated for {email}: {otp}");

            await _emailSender.SendEmailAsync(email, "Password Reset OTP", $"Your OTP is: {otp}");

            return true;
        }

        // STEP 2: Verify OTP and Reset Password
        public async Task<bool> VerifyOtpAndResetPasswordAsync(string email, string otp, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            if (user.OTPCode != otp || user.OTPExpiry == null || user.OTPExpiry < DateTime.UtcNow)
            {
                Console.WriteLine($"[DEBUG] OTP invalid or expired for {email}");
                return false;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            Console.WriteLine(result.Succeeded
                ? $"[DEBUG] Password reset successful for {email}"
                : $"[DEBUG] Password reset failed for {email}");

            foreach (var error in result.Errors)
            {
                Console.WriteLine($"[ERROR] {error.Description}");
            }

            if (result.Succeeded)
            {
                user.OTPCode = null;
                user.OTPExpiry = null;
                await _userManager.UpdateAsync(user);
            }

            return result.Succeeded;
        }
    }
}
