using ECommerce.BasketService.API.Domain.Entities;
using ECommerce.BasketService.API.DTOs;
using Space.Abstraction.Contracts;

namespace ECommerce.BasketService.API.UseCases.Commands;

public record AddItemCommand(
    Guid UserId,
    BasketItem BasketItem
    ) : IRequest<Basket>;
