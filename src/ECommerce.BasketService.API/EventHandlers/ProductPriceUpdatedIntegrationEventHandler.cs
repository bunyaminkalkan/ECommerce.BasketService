using ECommerce.BasketService.API.Repositories;
using ECommerce.BuildingBlocks.EventBus.Base.Abstractions;
using ECommerce.BuildingBlocks.EventBus.Base.Events.Catalogs;
using MassTransit;

namespace ECommerce.BasketService.API.EventHandlers;

public class ProductPriceUpdatedIntegrationEventHandlerT(IBasketRepository repo) : IIntegrationEventHandler<ProductPriceUpdatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductPriceUpdatedIntegrationEvent> context)
    {
        await repo.UpdateProductPriceAsync(context.Message.ProductId, context.Message.NewPrice);
    }
}
