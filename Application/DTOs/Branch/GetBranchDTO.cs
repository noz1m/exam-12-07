namespace Application.DTOs.Branch;

public class GetBranchDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}
