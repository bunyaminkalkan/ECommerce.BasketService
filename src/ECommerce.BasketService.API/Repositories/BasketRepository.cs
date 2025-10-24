using ECommerce.BasketService.API.Domain.Entities;
using ECommerce.BuildingBlocks.Shared.Kernel.ValueObjects;
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

    public async Task UpdateProductPriceAsync(Guid productId, Money newPrice)
    {
        var userKeys = await GetAllBasketKeysAsync();

        foreach (var key in userKeys)
        {
            var val = await _db.StringGetAsync(key);
            if (val.IsNullOrEmpty) continue;

            var basket = JsonSerializer.Deserialize<Basket>(val!, _jsonOptions);
            if (basket == null) continue;

            var itemsToUpdate = basket.Items.Where(i => i.ProductId == productId).ToList();

            if (itemsToUpdate.Any())
            {
                foreach (var item in itemsToUpdate)
                {
                    item.UnitPrice = newPrice;
                }

                var json = JsonSerializer.Serialize(basket, _jsonOptions);
                var ttl = await _db.KeyTimeToLiveAsync(key);
                await _db.StringSetAsync(key, json, ttl ?? _defaultTtl);
            }
        }
    }

    public async Task UpdateProductStockAsync(Guid productId, int quantityChange)
    {
        var userKeys = await GetAllBasketKeysAsync();

        foreach (var key in userKeys)
        {
            var val = await _db.StringGetAsync(key);
            if (val.IsNullOrEmpty) continue;

            var basket = JsonSerializer.Deserialize<Basket>(val!, _jsonOptions);
            if (basket == null) continue;

            var itemsToUpdate = basket.Items.Where(i => i.ProductId == productId).ToList();

            if (itemsToUpdate.Any())
            {
                foreach (var item in itemsToUpdate)
                {
                    item.UnitsInStock += quantityChange;
                }

                var json = JsonSerializer.Serialize(basket, _jsonOptions);
                var ttl = await _db.KeyTimeToLiveAsync(key);
                await _db.StringSetAsync(key, json, ttl ?? _defaultTtl);
            }
        }
    }

    private async Task<List<string>> GetAllBasketKeysAsync()
    {
        var keys = new List<string>();
        var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());

        await foreach (var key in server.KeysAsync(pattern: "basket:*"))
        {
            keys.Add(key.ToString());
        }

        return keys;
    }
}