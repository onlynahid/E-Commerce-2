using AYYUAZ.APP.Application.Exceptions.AppException;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using KeyNotFoundException = AYYUAZ.APP.Application.Exceptions.AppException.KeyNotFoundException;
// using KeyNotFoundException = AYYUAZ.APP.Application.Exceptions.AppException.KeyNotFoundException;

namespace AYYUAZ.APP.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred: {ExceptionType} - {Message}", 
                    ex.GetType().Name, ex.Message);

                var (statusCode, errorCode) = Resolve(ex);

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    errorCode,
                    message = MessageProvider(errorCode),
                    traceId = context.TraceIdentifier,
                    timestamp = DateTime.UtcNow
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
            }
        }

        private static (int statusCode, string errorCode) Resolve(Exception ex)
        {
            if (ex is AppException appEx)
                return (appEx.StatusCode, appEx.ErrorCode);

            return ex switch
            {
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "unauthorized"),
                ArgumentNullException => (StatusCodes.Status400BadRequest, "bad_request"),
                ArgumentException => (StatusCodes.Status400BadRequest, "bad_request"),
                InvalidOperationException => (StatusCodes.Status400BadRequest, "bad_request"),
              KeyNotFoundException => (StatusCodes.Status404NotFound, "not_found"),
                NotSupportedException => (StatusCodes.Status400BadRequest, "bad_request"),
                FormatException => (StatusCodes.Status400BadRequest, "bad_request"),
                _ => (StatusCodes.Status500InternalServerError, "server_error")
            };
        }
        private static string MessageProvider(string errorCode) => errorCode switch
        {
            "not_found" => "Tapılmadı.",
            "unauthorized" => "İcazəniz yoxdur.",
            "validation_error" => "Göndərilən məlumatlarda xəta var.",
            "bad_request" => "Yanlış sorğu göndərildi.",
            "server_error" => "Server xətası baş verdi.",
            "forbidden" => "Bu əməliyyatı yerinə yetirmək üçün icazəniz yoxdur.",
            _ => "Xəta baş verdi."
        };
    }
}