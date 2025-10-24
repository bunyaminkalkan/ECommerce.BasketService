using ECommerce.BuildingBlocks.Shared.Kernel.ValueObjects;

namespace ECommerce.BasketService.API.DTOs;

public class BasketItem
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = default!;
    public int Quantity { get; set; }
    public string? PictureUrl { get; set; }

    public Money UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
}
