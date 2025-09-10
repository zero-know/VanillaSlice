
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ErrorHandlerMiddleware> _logger;
    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            string errorId = Guid.NewGuid().ToString().ToLower();
            var response = context.Response;
            response.ContentType = "application/json";
            if (exception.InnerException != null)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
            }

            switch (exception)
            {
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            _logger.LogError(exception.Message);
            await response.WriteAsync(exception.Message);


        }
    }
}