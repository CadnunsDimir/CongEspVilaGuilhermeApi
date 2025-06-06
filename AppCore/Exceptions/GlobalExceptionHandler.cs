using CongEspVilaGuilhermeApi.AppCore.Exceptions;
using CongEspVilaGuilhermeApi.Domain.Services;
using Microsoft.AspNetCore.Diagnostics;

namespace CongEspVilaGuilhermeApi.AppCore.Exceptions;

internal class GlobalExceptionHandler : IExceptionHandler
{
    private ILoggerService logger;

    public GlobalExceptionHandler(ILoggerService logger)
    {
        this.logger = logger;
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.Log(exception);
        var exceptionsToShowReason = new[] { "InvalidOperation", "Argument" };
        var error = new Error
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = exception.GetType().Name.Replace("Exception", string.Empty),
        };

        if (exceptionsToShowReason.Contains(error.Title))
        {
            error.Status = StatusCodes.Status422UnprocessableEntity;
            error.Title += $", {exception.Message}";
        }

        await httpContext.Response.WriteAsJsonAsync(error, cancellationToken);

        return true;
    }
}