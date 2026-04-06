using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Application.Mappings;
using AYYUAZ.APP.Application.Services;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Infrastructure.Data;
using AYYUAZ.APP.Infrastructure.Repositories;
using AYYUAZ.APP.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Infrastructure.DI.Infrastructure
{
    public static class DependecyInjectionInfrastructure
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IAboutRepository, AboutRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IHeroRepository, HeroRepository>();


            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(InfrastructureAssemblyMarker).Assembly));

            services.AddScoped<IFileStorageService, FileStorageService>();

            return services;
        }
    }
}
