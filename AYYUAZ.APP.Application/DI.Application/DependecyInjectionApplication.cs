using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Application.Services;
using AYYUAZ.APP.Application.Service;
using AYYUAZ.APP.Infrastructure.Services;
using AYYUAZ.APP.Domain.Entities;

namespace AYYUAZ.APP.Application.DI.Application
{
    public static class DependecyInjectionApplication
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAboutService, AboutService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IHeroService, HeroService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}
