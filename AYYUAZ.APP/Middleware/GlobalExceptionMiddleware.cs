using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next,
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
                _logger.LogError(ex, "Unhandled exception occurred");

                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = GetMessage(ex)
                };

                context.Response.StatusCode = GetStatusCode(ex);

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response));
            }
        }
        private int GetStatusCode(Exception ex)
        {
            return ex switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };
        }
        private string GetMessage(Exception ex)
        {
            return ex switch
            {
                KeyNotFoundException => "Tapılmadı",
                UnauthorizedAccessException => "İcazəniz yoxdur",
                _ => "Server xətası baş verdi"
            };
        }
    }
}
