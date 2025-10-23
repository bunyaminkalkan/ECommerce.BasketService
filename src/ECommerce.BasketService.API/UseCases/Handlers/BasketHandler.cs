using ECommerce.BasketService.API.Domain.Entities;
using ECommerce.BasketService.API.Repositories;
using ECommerce.BasketService.API.UseCases.Commands;
using ECommerce.BasketService.API.UseCases.Queries;
using ECommerce.BuildingBlocks.EventBus.Base.Abstractions;
using ECommerce.BuildingBlocks.EventBus.Base.DTOs;
using ECommerce.BuildingBlocks.EventBus.Base.Events.Baskets;
using ECommerce.BuildingBlocks.Shared.Kernel.Exceptions;
using Space.Abstraction;
using Space.Abstraction.Attributes;
using Space.Abstraction.Context;

namespace ECommerce.BasketService.API.UseCases.Handlers;

public class BasketHandler(IBasketRepository repo, IEventBus eventBus)
{
    [Handle]
    public async Task<Nothing> CheckoutAsync(HandlerContext<CheckoutCommand> ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        var user = ctx.Request.User;
        var request = ctx.Request.CheckoutRequest;

        var basket = await repo.GetAsync(user.UserId);

        // Event yayınla
        var basketCheckedOutEvent = new BasketCheckedOutIntegrationEvent
        {
            OccurredOn = DateTime.UtcNow,
            CorrelationId = Guid.CreateVersion7(),
            CustomerId = user.UserId,
            CustomerEmail = user.CustomerEmail,
            CustomerName = user.CustomerName,
            Items = basket.Items.Select(i => new BasketItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList(),
            ShippingAddress = request.ShippingAddress,
            BillingAddress = request.BillingAddress,
        };

        await eventBus.PublishAsync(basketCheckedOutEvent);

        return Nothing.Value;
    }

    [Handle]
    public async Task<Basket?> GetUserItemsAsync(HandlerContext<GetUserItemsQuery> ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();
        return await repo.GetAsync(ctx.Request.UserId);
    }

    [Handle]
    public async Task<Basket> AddItemsAsync(HandlerContext<AddItemCommand> ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        var userId = ctx.Request.UserId;
        var basketItem = ctx.Request.BasketItem;

        var basket = await repo.GetAsync(userId) ?? new Basket { UserId = userId };
        var existing = basket.Items.FirstOrDefault(i => i.ProductId == basketItem.ProductId);

        if (existing != null)
            throw new BadRequestException("The product is already in your basket.");
        else
            basket.Items.Add(basketItem);

        await repo.SetAsync(basket);

        return basket;
    }

    [Handle]
    public async Task<Basket> UpdateItemAsync(HandlerContext<UpdateItemCommand> ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        var userId = ctx.Request.UserId;
        var productId = ctx.Request.ProductId;

        var basket = await repo.GetAsync(userId);
        if (basket == null) throw new BadRequestException("Basket cannot be found");

        var existing = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing == null) throw new BadRequestException("Product cannot be found");

        existing.Quantity = ctx.Request.Quantity;
        await repo.SetAsync(basket);

        return basket;
    }

    [Handle]
    public async Task<Nothing> RemoveItemAsync(HandlerContext<RemoveItemCommand> ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        var userId = ctx.Request.UserId;
        var productId = ctx.Request.ProductId;

        var basket = await repo.GetAsync(userId)
            ?? throw new BadRequestException("Basket not be found");

        basket.Items.RemoveAll(i => i.ProductId == productId);
        await repo.SetAsync(basket);

        return Nothing.Value;
    }

    [Handle]
    public async Task<Nothing> ClearBasketAsync(HandlerContext<ClearBasketCommand> ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();
        await repo.RemoveAsync(ctx.Request.UserId);
        return Nothing.Value;
    }
}