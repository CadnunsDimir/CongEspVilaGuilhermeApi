using CongEspVilaGuilhermeApi.Domain.Entities;

namespace CongEspVilaGuilhermeApi.Domain.Services
{
    public interface IEmailService
    {
        Task NotifyNewUserAsync(User user);
        Task SendNewPasswordAsync(User user, string plainPassword);
        Task SendResetPassordEmailAsync(User user);
    }
}
