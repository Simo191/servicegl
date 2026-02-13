using MultiServices.Domain.Entities.Restaurant;
using MultiServices.Domain.Enums;

namespace MultiServices.Domain.Interfaces.Repositories;

public interface IRestaurantRepository : IGenericRepository<RestaurantEntity>
{
    Task<IReadOnlyList<RestaurantEntity>> SearchAsync(string? query, CuisineType? cuisine, PriceRange? price, 
        double? maxDistance, double? lat, double? lng, double? minRating, int page, int pageSize, CancellationToken ct = default);
    Task<RestaurantEntity?> GetWithMenuAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<RestaurantEntity>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default);
    Task<IReadOnlyList<RestaurantEntity>> GetNearbyAsync(double lat, double lng, double radiusKm, CancellationToken ct = default);
    Task<IReadOnlyList<RestaurantEntity>> GetPopularAsync(int count, CancellationToken ct = default);
}
