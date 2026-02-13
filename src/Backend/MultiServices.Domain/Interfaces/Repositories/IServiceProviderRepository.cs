using MultiServices.Domain.Entities.Service;
using MultiServices.Domain.Enums;

namespace MultiServices.Domain.Interfaces.Repositories;

public interface IServiceProviderRepository : IGenericRepository<ServiceProvider>
{
    Task<IReadOnlyList<ServiceProvider>> SearchAsync(ServiceCategory? category, double? minRating,
        string? city, decimal? maxPrice, int page, int pageSize, CancellationToken ct = default);
    Task<ServiceProvider?> GetWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceProvider>> GetAvailableAsync(ServiceCategory category, DateTime date, string city, CancellationToken ct = default);
}
