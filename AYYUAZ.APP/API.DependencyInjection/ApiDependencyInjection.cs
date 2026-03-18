using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Infrastructure.Data;
using AYYUAZ.APP.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AYYUAZ.APP.Infrastructure.ApplicationUser;

namespace AYYUAZ.APP.API.DependencyInjection
{
    public static class ApiDependencyInjection
    {
        public static IServiceCollection AddApi(this IServiceCollection services,IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var frontendUrl = configuration["Frontend:Url"];

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("DefaultConnection string is not configured.");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy.WithOrigins(frontendUrl)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // services.AddGlobalExceptionFilter();

            services.AddControllers(options =>
            {
                // options.Filters.Add<GlobalExceptionFilter>();
            });
            
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}
