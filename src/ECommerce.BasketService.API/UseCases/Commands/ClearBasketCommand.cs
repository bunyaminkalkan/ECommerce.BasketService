using Space.Abstraction;
using Space.Abstraction.Contracts;

namespace ECommerce.BasketService.API.UseCases.Commands;

public record ClearBasketCommand(Guid UserId) : IRequest<Nothing>;
