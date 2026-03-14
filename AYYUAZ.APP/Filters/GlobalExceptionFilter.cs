using AYYUAZ.APP.Application.Exceptions.AppException;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
// using KeyNotFoundException = AYYUAZ.APP.Application.Exceptions.AppException.KeyNotFoundException;

namespace AYYUAZ.APP.Filters
{
    // public class GlobalExceptionFilter : IExceptionFilter
    // {
    //     private readonly ILogger<GlobalExceptionFilter> _logger;

    //     public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    //     {
    //         _logger = logger;
    //     }

    //     public void OnException(ExceptionContext context)
    //     {
    //         _logger.LogError(context.Exception, 
    //             "Exception in action {ActionName} of controller {ControllerName}", 
    //             context.ActionDescriptor.DisplayName,
    //             context.RouteData.Values["controller"]);

    //         var (statusCode, errorCode, message) = ResolveException(context.Exception);

    //         var result = new ObjectResult(new
    //         {
    //             errorCode,
    //             message,
    //             controller = context.RouteData.Values["controller"],
    //             action = context.RouteData.Values["action"],
    //             traceId = context.HttpContext.TraceIdentifier,
    //             timestamp = DateTime.UtcNow
    //         })
    //         {
    //             StatusCode = statusCode
    //         };

    //         context.Result = result;
    //         context.ExceptionHandled = true;
    //     }

    //     private static (int statusCode, string errorCode, string message) ResolveException(Exception ex)
    //     {
    //         return ex switch
    //         {
    //             NotFoundException => (StatusCodes.Status404NotFound, "not_found", "Tap?lmad?."),
    //             BadRequestException => (StatusCodes.Status400BadRequest, "validation_error", "G?nd?ril?n m?lumatlarda x?ta var."),
    //             ForbiddenException => (StatusCodes.Status403Forbidden, "forbidden", "Bu ?m?liyyat? yerin? yetirm?k ???n icaz?niz yoxdur."),
    //             UnauthorizedException => (StatusCodes.Status401Unauthorized, "unauthorized", "?caz?niz yoxdur."),
                
    //             ArgumentNullException => (StatusCodes.Status400BadRequest, "bad_request", "M?cburi sah? bo? burax?la bilm?z."),
    //             ArgumentException => (StatusCodes.Status400BadRequest, "bad_request", "Yanl?? parametr d?y?ri."),
    //             UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "unauthorized", "?caz?niz yoxdur."),
    //             KeyNotFoundException => (StatusCodes.Status404NotFound, "not_found", "Tap?lmad?."),
    //             InvalidOperationException => (StatusCodes.Status400BadRequest, "bad_request", "Yanl?? ?m?liyyat."),
                
    //             _ => (StatusCodes.Status500InternalServerError, "server_error", "Server x?tas? ba? verdi.")
    //         };
    //     }
    // }
    // public static class GlobalExceptionFilterExtensions
    // {
    //     public static IServiceCollection AddGlobalExceptionFilter(this IServiceCollection services)
    //     {
    //         services.AddScoped<GlobalExceptionFilter>();
    //         return services;
    //     }
    // }
}