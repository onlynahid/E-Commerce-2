using AYYUAZ.APP.Infrastructure.Data;
using AYYUAZ.APP.Infrastructure.Services;

namespace AYYUAZ.APP.Initalizers
{
    public static class ApplicationInitializers
    {
        public static async Task InitializeApplicationAsync(this WebApplication app)
        {
            var wwwrootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
            if (!Directory.Exists(wwwrootPath))
            {
                Directory.CreateDirectory(wwwrootPath);
            }

            var uploadsPath = Path.Combine(wwwrootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
                Directory.CreateDirectory(Path.Combine(uploadsPath, "products"));
                Directory.CreateDirectory(Path.Combine(uploadsPath, "categories"));
                Directory.CreateDirectory(Path.Combine(uploadsPath, "users"));
            }

            CreateDefaultImages(uploadsPath);

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                var logger = services.GetRequiredService<ILogger<Program>>();

                context.Database.EnsureCreated();
                await DataSeeder.SeedInitialData(context, services);

                logger.LogInformation("Database initialized and seeded successfully.");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private static void CreateDefaultImages(string uploadsPath)
        {
            var productImagePath = Path.Combine(uploadsPath, "products", "default-product.jpg");
            if (!File.Exists(productImagePath))
            {
                CreatePlaceholderImage(productImagePath, "Product");
            }

            var categoryImagePath = Path.Combine(uploadsPath, "categories", "default-category.jpg");
            if (!File.Exists(categoryImagePath))
            {
                CreatePlaceholderImage(categoryImagePath, "Category");
            }
        }

        private static void CreatePlaceholderImage(string filePath, string text)
        {
            var svgContent = $@"<svg width=""400"" height=""300"" xmlns=""http://www.w3.org/2000/svg"">
            <rect width=""100%"" height=""100%"" fill=""#f0f0f0""/>
            <text x=""50%"" y=""50%"" text-anchor=""middle"" dy="".3em"" font-family=""Arial, sans-serif"" font-size=""24"" fill=""#666"">{text} Image</text>
        </svg>";

            var svgPath = Path.ChangeExtension(filePath, ".svg");
            File.WriteAllText(svgPath, svgContent);

            var textPath = Path.ChangeExtension(filePath, ".txt");
            File.WriteAllText(textPath, $"Placeholder for {text} image");
        }
    }
}
