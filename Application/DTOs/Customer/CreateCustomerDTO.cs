namespace Application.DTOs.Customer;

public class CreateCustomerDTO
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string IdentityUserId { get; set; } = string.Empty;
}
