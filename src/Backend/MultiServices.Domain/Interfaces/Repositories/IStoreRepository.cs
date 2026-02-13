using MultiServices.Domain.Entities.Grocery;

namespace MultiServices.Domain.Interfaces.Repositories;

public interface IStoreRepository : IGenericRepository<GroceryStoreEntity>
{
    Task<IReadOnlyList<GroceryStoreEntity>> SearchAsync(string? query, double? maxDistance, double? lat, double? lng,
        bool? hasPromo, bool? freeDelivery, int page, int pageSize, CancellationToken ct = default);
    Task<GroceryStoreEntity?> GetWithCatalogAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<GroceryProduct>> SearchProductsAsync(Guid storeId, string? query, Guid? categoryId,
        string? brand, bool? isBio, bool? isHalal, bool? onPromo, int page, int pageSize, CancellationToken ct = default);
}
