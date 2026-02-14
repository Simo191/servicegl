using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using MultiServices.Domain.Interfaces.Services;
using MultiServices.Infrastructure.Data;
using MultiServices.Infrastructure.Data.Interceptors;

namespace MultiServices.Infrastructure;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var services = new ServiceCollection();

        services.AddSingleton<ICurrentUserService, DesignTimeCurrentUserService>();
        services.AddSingleton<AuditableEntityInterceptor>();

        var cs = Environment.GetEnvironmentVariable("DefaultConnection")
                 ?? "Host=localhost;Port=5432;Database=multiservices;Username=postgres;Password=123456";

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(cs);
            options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
        });

        return services.BuildServiceProvider().GetRequiredService<ApplicationDbContext>();
    }

    private sealed class DesignTimeCurrentUserService : ICurrentUserService
    {
        public string? UserId => null;
        public string? Email => null;
        public string? Role => null;
        public bool IsAuthenticated => false;
    }
}
