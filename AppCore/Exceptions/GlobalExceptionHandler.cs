using CongEspVilaGuilhermeApi.AppCore.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

internal class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var error = new Error
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = exception.GetType().Name.Replace("Exception", string.Empty),
        };

        if(error.Title == "Argument")
        {
            error.Title += $", {exception.Message}";
        }

        await httpContext.Response.WriteAsJsonAsync(error, cancellationToken);

        return true;
    }
}