using Business.Models.Dtos.Create;

namespace Business.Models.Dtos.Update;

public class ProjectUpdateForm : ProjectCreateForm
{
    public int Id { get; set; }
}