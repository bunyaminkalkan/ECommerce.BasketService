using ECommerce.BasketService.API.Domain.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.BasketService.API.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDatabase _db;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly TimeSpan _defaultTtl = TimeSpan.FromDays(30);

    public BasketRepository(IConnectionMultiplexer muxer)
    {
        _db = muxer.GetDatabase();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    private static string Key(Guid userId) => $"basket:{userId}";

    public async Task<Basket?> GetAsync(Guid userId)
    {
        var key = Key(userId);
        var val = await _db.StringGetAsync(key);
        if (val.IsNullOrEmpty) return null;

        var basket = JsonSerializer.Deserialize<Basket>(val!, _jsonOptions);

        await _db.KeyExpireAsync(key, _defaultTtl);

        return basket;
    }

    public async Task SetAsync(Basket basket, TimeSpan? ttl = null)
    {
        var json = JsonSerializer.Serialize(basket, _jsonOptions);
        await _db.StringSetAsync(Key(basket.UserId), json, ttl ?? _defaultTtl);
    }

    public async Task RemoveAsync(Guid userId)
    {
        await _db.KeyDeleteAsync(Key(userId));
    }
}
