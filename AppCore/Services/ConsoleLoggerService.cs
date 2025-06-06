using CongEspVilaGuilhermeApi.Domain.Services;
using System;
using System.Diagnostics;

namespace CongEspVilaGuilhermeApi.AppCore.Services
{
    public class ConsoleLoggerService : ILoggerService
    {
        public void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - {message}");
        }

        public void Log(Exception exception)
        {
            var exceptionName = exception.GetType().Name;
            var frame = new StackTrace(exception, true).GetFrame(0);
            var functionSignature = exception.StackTrace?.Split(" in ").FirstOrDefault()?.Replace(" at ", string.Empty).Trim();
            var fileLocation = $"{frame?.GetFileName()}:{frame?.GetFileLineNumber()}";
            Log($"[{ exceptionName }] { exception.Message } " +
                $"\n\tCause :{ functionSignature }" +
                $"\n\t\tin { fileLocation }");
        }
    }
}
