namespace Business.Models.Dtos.Create;

public class ProjectCreateForm
{
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int UserId { get; set; }
    public int StatusId { get; set; }
    public int CustomerId { get; set; }
    public int ServiceId { get; set; }

    public string CustomerName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
}
