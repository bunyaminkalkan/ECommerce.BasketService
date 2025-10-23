using ECommerce.BuildingBlocks.Shared.Kernel.ValueObjects;

namespace ECommerce.BasketService.API.DTOs;

public class CheckoutRequest
{
    public Address ShippingAddress { get; set; }
    public Address BillingAddress { get; set; }
}
