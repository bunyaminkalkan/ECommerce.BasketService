using ECommerce.BasketService.API.Domain.Entities;
using ECommerce.BuildingBlocks.Shared.Kernel.ValueObjects;

namespace ECommerce.BasketService.API.Repositories;

public interface IBasketRepository
{
    Task<Basket?> GetAsync(Guid userId);
    Task SetAsync(Basket basket, TimeSpan? ttl = null);
    Task RemoveAsync(Guid userId);
    Task UpdateProductPriceAsync(Guid productId, Money newPrice);
    Task UpdateProductStockAsync(Guid productId, int newStock);
}
