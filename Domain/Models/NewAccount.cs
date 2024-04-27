
using CongEspVilaGuilhermeApi.Domain.Entities;

namespace CongEspVilaGuilhermeApi.Domain.Models
{
    public class NewAccount
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        internal User CreateUserEntity(string passwordHash)
        {
            return new User
            {
                UserName = UserName,
                Email = Email,
                PasswordHash = passwordHash,
                Role = User.DefaultRole
            };
        }
    }
}
