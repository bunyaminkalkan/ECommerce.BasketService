using ECommerce.BasketService.API.DTOs;
using Space.Abstraction;
using Space.Abstraction.Contracts;

namespace ECommerce.BasketService.API.UseCases.Commands;

public record CheckoutCommand(
    UserDto User,
    CheckoutRequest CheckoutRequest
    ) : IRequest<Nothing>;
