namespace ECommerce.BasketService.API.DTOs;

public class UserDto
{
    public Guid UserId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }

    public UserDto(Guid userId, string customerName, string customerEmail)
    {
        UserId = userId;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
    }
}
