using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MultiServices.Domain.Interfaces.Services;

namespace MultiServices.Infrastructure.Services.Cache;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        var options = new DistributedCacheEntryOptions();
        if (ttl.HasValue)
            options.SetAbsoluteExpiration(ttl.Value);

        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, json, options, ct);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var json = await _cache.GetStringAsync(key, ct);
        if (string.IsNullOrWhiteSpace(json))
            return default;

        return JsonSerializer.Deserialize<T>(json);
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
        => _cache.RemoveAsync(key, ct);

    public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
