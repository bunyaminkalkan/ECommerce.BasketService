using ECommerce.BasketService.API.Repositories;
using ECommerce.BuildingBlocks.Shared.Kernel.Auth.Options;
using ECommerce.BuildingBlocks.Shared.Kernel.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Space.DependencyInjection;
using StackExchange.Redis;

namespace ECommerce.BasketService.API;

public static class DependecyInjection
{
    public static IServiceCollection InstallServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        #region OpenApi
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "JWT Authorization header using the Bearer scheme."
                    }
                };

                document.SecurityRequirements = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
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
                            new string[] {}
                        }
                    }
                };

                return Task.CompletedTask;
            });
        });
        #endregion

        #region Redis
        services.AddSingleton<IConnectionMultiplexer>(
            sp => ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!)
            );
        #endregion

        #region Space
        services.AddSpace(configuration =>
        {
            configuration.ServiceLifetime = ServiceLifetime.Scoped;
        });
        #endregion

        #region Interfaces
        services.AddScoped<IBasketRepository, BasketRepository>();
        #endregion

        #region Exceptions
        services.AddScoped<ExceptionMiddleware>();
        #endregion

        #region Auth
        services.AddHttpContextAccessor();

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.Configure<JwtOptions>(configuration.GetSection("JwtSettings"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer();

        services.AddAuthorization();
        #endregion

        return services;
    }
}
