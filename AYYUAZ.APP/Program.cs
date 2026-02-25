using AYYUAZ.APP.Application.DI.Application;
using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Application.Service;
using AYYUAZ.APP.Application.Services;
using AYYUAZ.APP.Domain.Entities;
using AYYUAZ.APP.Domain.Interfaces;
using AYYUAZ.APP.Infrastructure.Data;
using AYYUAZ.APP.Infrastructure.DI.Infrastructure;
using AYYUAZ.APP.Infrastructure.Repositories;
using AYYUAZ.APP.Infrastructure.Services;
using AYYUAZ.APP.Middleware;
using AYYUAZ.APP.ServiceExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using GlobalExceptionMiddleware = AYYUAZ.APP.Middleware.GlobalExceptionMiddleware;
namespace AYYUAZ.APP
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
          https://aka.ms/aspnetcore/swashbuckle
          
     
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });
          
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();           
            #region
            // Repository registrations
            //builder.Services.                                     AddScoped<IProductRepository, ProductRepository>();
            //builder.Services                                      .AddScoped<IOrderRepository, OrderRepository>();
            //builder.Services                                     .AddScoped<ISettingsRepository, SettingsRepository>();
            //builder.Services                                          .AddScoped<ICategoryRepository, CategoryRepository>();
            //builder.Services                                           .AddScoped<IAboutRepository, AboutRepository>();
            //builder.Services                                           .AddScoped<IUserRepository, UserRepository>();
            //builder.Services.                                               AddScoped<IHeroRepository, HeroRepository>();


            // Service registrations
            //builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            //builder.Services.AddScoped<IProductService, ProductService>();
            //builder.Services.AddScoped<IOrderService, OrderService>();
            //builder.Services.AddScoped<ISettingsService, SettingsService>();
            //builder.Services.AddScoped<ICategoryService, CategoryService>();
            //builder.Services.AddScoped<IAboutService, AboutService>();
            //builder.Services.AddScoped<IAuthService, AuthService>();
            //builder.Services.AddScoped<IJwtService, JwtService>();
            ////builder.Services.AddScoped<IBasketService, BasketService>();
            //builder.Services.AddScoped<IHeroService, HeroService>();
            //builder.Services.AddScoped<GlobalExceptionFilter>();
            #endregion
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddProjectServices(builder.Configuration);

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("DefaultConnection string is not configured");

            var app = builder.Build();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            await InitializeApplication(app);

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
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.MapControllers();
            app.Run();
        }
        private static async Task InitializeApplication(WebApplication app)
        {
            // Ensure wwwroot directory exists for static files
            var wwwrootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
            if (!Directory.Exists(wwwrootPath))
            {
                Directory.CreateDirectory(wwwrootPath);
            }

            // Ensure upload directories exist
            var uploadsPath = Path.Combine(wwwrootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
                Directory.CreateDirectory(Path.Combine(uploadsPath, "products"));
                Directory.CreateDirectory(Path.Combine(uploadsPath, "categories"));
                Directory.CreateDirectory(Path.Combine(uploadsPath, "users"));
            }

            // Create default placeholder images
            CreateDefaultImages(uploadsPath);

            // Database initialization and seeding
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    var logger = services.GetRequiredService<ILogger<Program>>();

                    // Ensure database is created
                    context.Database.EnsureCreated();

                    // Seed initial data with service provider - await the async call
                    await DataSeeder.SeedInitialData(context, services);

                    logger.LogInformation("Database initialized and seeded successfully.");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
        }
        private static void CreateDefaultImages(string uploadsPath)
        {
            // Create default product image placeholder
            var productImagePath = Path.Combine(uploadsPath, "products", "default-product.jpg");
            if (!File.Exists(productImagePath))
            {
                CreatePlaceholderImage(productImagePath, "Product");
            }

            // Create default category image placeholder
            var categoryImagePath = Path.Combine(uploadsPath, "categories", "default-category.jpg");
            if (!File.Exists(categoryImagePath))
            {
                CreatePlaceholderImage(categoryImagePath, "Category");
            }
        }
        private static void CreatePlaceholderImage(string filePath, string text)
        {
            // Create a simple SVG placeholder that browsers can display
            var svgContent = $@"<svg width=""400"" height=""300"" xmlns=""http://www.w3.org/2000/svg"">
                <rect width=""100%"" height=""100%"" fill=""#f0f0f0""/>
                <text x=""50%"" y=""50%"" text-anchor=""middle"" dy="".3em"" font-family=""Arial, sans-serif"" font-size=""24"" fill=""#666"">{text} Image</text>
            </svg>";

            // Save as SVG file (browsers can display SVG directly)
            var svgPath = Path.ChangeExtension(filePath, ".svg");
            File.WriteAllText(svgPath, svgContent);

            // Also create a simple text file as backup
            var textPath = Path.ChangeExtension(filePath, ".txt");
            File.WriteAllText(textPath, $"Placeholder for {text} image");
        }
    }
}