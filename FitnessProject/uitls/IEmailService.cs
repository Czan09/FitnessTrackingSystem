using System.Threading.Tasks;
namespace FitnessProject.uitls
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
