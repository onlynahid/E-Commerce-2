using AYYUAZ.APP.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace AYYUAZ.APP.ServiceExtensions
{
    public static class ProjectServiceExtension
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AYYUAZ.APP API",
                    Version = "v1",
                    Description = "API for AYYUAZ Application"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
                c.MapType<IFormFile>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                });
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:3000", "https://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
          .AddJwtBearer(options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                  ValidateIssuer = true,
                  ValidIssuer = jwtIssuer,
                  ValidateAudience = true,
                  ValidAudience = jwtAudience,
                  ValidateLifetime = true,
                  ClockSkew = TimeSpan.Zero
              };

              options.Events = new JwtBearerEvents
              {
                  OnMessageReceived = context =>
                  {
                      var logger = context.HttpContext.RequestServices
                          .GetRequiredService<ILoggerFactory>()
                          .CreateLogger("JwtDebug");

                      var authHeader = context.Request.Headers["Authorization"].ToString();

                      logger.LogDebug("JWT OnMessageReceived - Auth Header: {AuthHeader}",
                          string.IsNullOrEmpty(authHeader)
                              ? "MISSING"
                              : authHeader.Substring(0, Math.Min(authHeader.Length, 50)) + "...");

                      return Task.CompletedTask;
                  },

                  OnTokenValidated = context =>
                  {
                      var logger = context.HttpContext.RequestServices
                          .GetRequiredService<ILoggerFactory>()
                          .CreateLogger("JwtDebug");

                      var claims = context.Principal?.Claims?.ToList() ?? new List<System.Security.Claims.Claim>();
                      logger.LogDebug("JWT Token validated successfully. Claims count: {ClaimsCount}", claims.Count);

                      foreach (var claim in claims)
                          logger.LogDebug("JWT Claim: {Type} = {Value}", claim.Type, claim.Value);

                      return Task.CompletedTask;
                  },

                  OnAuthenticationFailed = context =>
                  {
                      var logger = context.HttpContext.RequestServices
                          .GetRequiredService<ILoggerFactory>()
                          .CreateLogger("JwtDebug");

                      logger.LogError(context.Exception, "JWT Authentication failed");
                      return Task.CompletedTask;
                  },

                  OnChallenge = context =>
                  {
                      var logger = context.HttpContext.RequestServices
                          .GetRequiredService<ILoggerFactory>()
                          .CreateLogger("JwtDebug");

                      logger.LogWarning("JWT Challenge triggered: {Error}, {ErrorDescription}",
                          context.Error, context.ErrorDescription);

                      return Task.CompletedTask;
                  }
              };
          });
            return services;

        } 
    }
}
