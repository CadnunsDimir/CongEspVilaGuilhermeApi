using CongEspVilaGuilhermeApi.Domain.Entities;

namespace CongEspVilaGuilhermeApi.Domain.Services
{
    public interface ITokenService
    {
        string GeneratePasswordHash(string plainPassword);
        string GenerateToken(User user);
    }
}