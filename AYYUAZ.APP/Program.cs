using AYYUAZ.APP.API.DependencyInjection;
using AYYUAZ.APP.Application.DI.Application;
using AYYUAZ.APP.Application.Mappings;
using AYYUAZ.APP.Infrastructure.DI.Infrastructure;
using AYYUAZ.APP.Initalizers;
using AYYUAZ.APP.Middleware;
using AYYUAZ.APP.ServiceExtensions;
using Microsoft.Extensions.DependencyInjection;
using GlobalExceptionMiddleware = AYYUAZ.APP.Middleware.GlobalExceptionMiddleware;

namespace AYYUAZ.APP
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddProjectServices(builder.Configuration);
            builder.Services.AddApi(builder.Configuration);
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AYYUAZ.APP API v1");
                    c.RoutePrefix = "swagger";
                });
            }
            app.UseHttpsRedirection();
            app.UseCors("FrontendPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.MapControllers();

            app.Run();
        }
    }
}