using ECommerce.BasketService.API.Repositories;
using ECommerce.BuildingBlocks.EventBus.Base.Abstractions;
using ECommerce.BuildingBlocks.EventBus.Base.Events.Inventories;
using MassTransit;

namespace ECommerce.BasketService.API.EventHandlers;

public class StockUpdatedIntegrationEventHandler(IBasketRepository repo) : IIntegrationEventHandler<StockUpdatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<StockUpdatedIntegrationEvent> context)
    {
        await repo.UpdateProductStockAsync(context.Message.ProductId, context.Message.QuantityChange);
    }
}
