using CongEspVilaGuilhermeApi.Domain.Entities.Preaching;

namespace CongEspVilaGuilhermeApi.Domain.Repositories
{
    public interface IPreachingScheduleRepository
    {
        /// <summary>
        /// Registra os dias fixos de pregação.
        /// </summary>
        /// <param name="fixedPreachingDay"dia fixo para pregação.</param>
        void createOrUpdate(FixedPreachingDay fixedPreachingDay);

        /// <summary>
        /// Obtém todos os dias fixos de pregação.
        /// </summary>
        /// <returns>Lista contendo os dias fixos</returns>
        List<FixedPreachingDay> GetAllFixedPreachingDays();
        
    }
}