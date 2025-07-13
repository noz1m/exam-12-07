using Application.Paginations;
namespace Application.Filters;

public class FilterCustomer : ValidFilter
{
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
