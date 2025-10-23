namespace ECommerce.BasketService.API.Controllers;

using ECommerce.BasketService.API.DTOs;
using ECommerce.BasketService.API.Repositories;
using ECommerce.BasketService.API.UseCases.Commands;
using ECommerce.BasketService.API.UseCases.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Space.Abstraction;
using System.Security.Claims;

[ApiController]
[Route("/[controller]")]
[Authorize(Roles = "Customer")]
public class BasketController(ISpace space, IBasketRepository repository, IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    [HttpPost("/checkout")]
    public async Task<IActionResult> CheckoutAsync([FromBody] CheckoutRequest request)
    {
        var userId = Guid.Parse(httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var customerName = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Name)!.Value;
        var customerEmail = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Email)!.Value;
        var user = new UserDto(userId, customerName, customerEmail);

        var command = new CheckoutCommand(user, request);
        var response = await space.Send(command);
        return Ok(response);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserItemsAsync([FromRoute] Guid userId)
    {
        var request = new GetUserItemsQuery(userId);
        var response = await space.Send(request);
        return Ok(response);
    }

    [HttpPost("{userId:guid}/items")]
    public async Task<IActionResult> AddItemAsync([FromRoute] Guid userId, [FromBody] BasketItem item)
    {
        var request = new AddItemCommand(userId, item);
        var response = await space.Send(request);
        return Ok(response);
    }

    [HttpPut("{userId}/items/{productId:guid}")]
    public async Task<IActionResult> UpdateItemAsync(Guid userId, Guid productId, [FromBody] UpdateQuantityRequest quantityRequest)
    {
        var request = new UpdateItemCommand(userId, productId, quantityRequest.Quantity);
        var response = await space.Send(request);
        return Ok(response);
    }

    [HttpDelete("{userId:guid}/items/{productId:guid}")]
    public async Task<IActionResult> RemoveItemAsync([FromRoute] Guid userId, [FromRoute] Guid productId)
    {
        var request = new RemoveItemCommand(userId, productId);
        var response = await space.Send(request);
        return Ok(response);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> ClearBasketAsync(Guid userId)
    {
        var request = new ClearBasketCommand(userId);
        await space.Send(request);
        return Ok();
    }
}

