using CongEspVilaGuilhermeApi.Domain.Entities;

namespace CongEspVilaGuilhermeApi.Domain.Services
{
    public interface IEmailService
    {
        void NotifyNewUser(User user);
        void SendNewPassword(User user, string plainPassword);
        void SendResetPassordEmail(User user);
    }
}
