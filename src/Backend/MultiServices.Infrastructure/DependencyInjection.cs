using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiServices.Domain.Entities.Identity;
using MultiServices.Domain.Interfaces.Services;
using MultiServices.Infrastructure.Data;
using MultiServices.Infrastructure.Data.Interceptors;
using MultiServices.Infrastructure.Services.Auth;
using MultiServices.Infrastructure.Services.Cache;
using MultiServices.Infrastructure.Services.Email;
using MultiServices.Infrastructure.Services.Sms;
using Npgsql;

namespace MultiServices.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<AuditableEntityInterceptor>();


        // ✅ Build NpgsqlDataSource with Dynamic JSON enabled
        var cs = configuration.GetConnectionString("DefaultConnection");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(cs);
        dataSourceBuilder.EnableDynamicJson(); // ✅ FIX
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                dataSource,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            );

            options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
        });

        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IEmailService, EmailService>();
        var redisConnection = configuration.GetConnectionString("Redis");

        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "MultiServices_";
            });
        }
        else
        {
            // ✅ Fournit IDistributedCache en mémoire
            services.AddDistributedMemoryCache();
        }


        services.AddScoped<ICacheService, CacheService>();


        // ✅ Identity : enregistre UserManager<ApplicationUser>
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        return services;
    }
}
