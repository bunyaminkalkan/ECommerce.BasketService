using ECommerce.BasketService.API.Domain.Entities;

namespace ECommerce.BasketService.API.Repositories;

public interface IBasketRepository
{
    Task<Basket?> GetAsync(Guid userId);
    Task SetAsync(Basket basket, TimeSpan? ttl = null);
    Task RemoveAsync(Guid userId);
}
