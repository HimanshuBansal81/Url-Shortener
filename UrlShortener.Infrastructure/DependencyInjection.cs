using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.Services;
using UrlShortener.Infrastructure.Authentication;
using UrlShortener.Infrastructure.Data;
using UrlShortener.Infrastructure.Health;
using UrlShortener.Infrastructure.Repositories;
using StackExchange.Redis;
using UrlShortener.Infrastructure.Cache;

namespace UrlShortener.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration["Redis:ConnectionString"];
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Database connection string is missing.");
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                    ?? throw new InvalidOperationException("JWT configuration is missing.");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Key)
                    ),
                    ClockSkew = TimeSpan.Zero
                };
            });
        services.AddAuthorization();

        var healthChecks = services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database", failureStatus: HealthStatus.Unhealthy);

        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            try
            {
                services.AddSingleton<IConnectionMultiplexer>(_ =>
                {
                    var options = ConfigurationOptions.Parse(redisConnection);
                    options.AbortOnConnectFail = false;
                    return ConnectionMultiplexer.Connect(options);
                });
                services.AddScoped<ICacheService, RedisCacheService>();
                healthChecks.AddCheck<RedisHealthCheck>("redis", failureStatus: HealthStatus.Unhealthy);
            }
            catch
            {
                services.AddScoped<ICacheService, NullCacheService>();
                services.AddSingleton<RedisHealthCheck>();
                healthChecks.AddCheck<RedisHealthCheck>("redis", failureStatus: HealthStatus.Unhealthy);
            }
        }
        else
        {
            services.AddScoped<ICacheService, NullCacheService>();
            services.AddSingleton<RedisHealthCheck>();
            healthChecks.AddCheck<RedisHealthCheck>("redis", failureStatus: HealthStatus.Unhealthy);
        }

        services.AddScoped<IUrlRepository, UrlRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUrlService, UrlService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
