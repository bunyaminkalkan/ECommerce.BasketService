using Space.Abstraction;
using Space.Abstraction.Contracts;

namespace ECommerce.BasketService.API.UseCases.Commands;

public record RemoveItemCommand(
    Guid UserId,
    Guid ProductId
    ) : IRequest<Nothing>;
