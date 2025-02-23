using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectEntity
{
    public int Id { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? EndDate { get; set; }

    public int UserId { get; set; }
    public virtual UserEntity User { get; set; } = null!;

    public int StatusId { get; set; }
    public virtual ActivityStatusEntity Status { get; set; } = null!;

    public int CustomerId { get; set; }
    public virtual CustomerEntity Customer { get; set; } = null!;

    public int ServiceId { get; set; }
    public virtual ServiceEntity Service { get; set; } = null!;

}
