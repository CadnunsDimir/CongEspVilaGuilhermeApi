
namespace CongEspVilaGuilhermeApi.Domain.Services
{
    public interface ILoggerService
    {
        void Log(string message);
        void Log(Exception exception);
    }
}
