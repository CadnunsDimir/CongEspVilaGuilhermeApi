using CongEspVilaGuilhermeApi.Domain.Entities;

namespace CongEspVilaGuilhermeApi.Domain.Repositories
{
    public interface ILifeAndMinistryRepository
    {
        Task CreateOrUpdate(LifeAndMinistryWeek week);
        Task<LifeAndMinistryWeek?> GetById(string weekId);
    }
}
