using ECommerce.BasketService.API.Domain.Entities;
using Space.Abstraction.Contracts;

namespace ECommerce.BasketService.API.UseCases.Commands;

public record UpdateItemCommand(
    Guid UserId,
    Guid ProductId,
    int Quantity
    ) : IRequest<Basket>;
