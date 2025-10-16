using ECommerce.BasketService.API.Domain.Entities;
using Space.Abstraction.Contracts;

namespace ECommerce.BasketService.API.UseCases.Queries;

public record GetUserItemsQuery(Guid UserId) : IRequest<Basket?>;
