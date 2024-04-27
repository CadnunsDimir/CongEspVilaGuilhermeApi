using CongEspVilaGuilhermeApi.Domain.Entities;

namespace CongEspVilaGuilhermeApi.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUserName(string userName);
        Task Create(User user);
        Task Update(User user);
        Task<bool> UserNameIsAvailable(string userName);
    }
}
