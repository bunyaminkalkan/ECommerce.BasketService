using ECommerce.BasketService.API.DTOs;

namespace ECommerce.BasketService.API.Domain.Entities;

public class Basket
{
    public Guid UserId { get; set; } = default!;
    public List<BasketItem> Items { get; set; } = new();
}
