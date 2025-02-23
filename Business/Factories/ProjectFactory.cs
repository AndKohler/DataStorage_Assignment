using Business.Models;
using Business.Models.Dtos.Create;
using Business.Models.Dtos.Update;
using Data.Entities;

namespace Business.Factories;

public static class ProjectFactory
{
    public static ProjectEntity Create(ProjectCreateForm form) => new()
    {
        ProjectName = form.ProjectName,
        Description = form.Description,
        StartDate = form.StartDate,
        EndDate = form.EndDate,
        UserId = form.UserId,
        StatusId = form.StatusId,
        CustomerId = form.CustomerId,
        ServiceId = form.ServiceId,
    };

    public static void Update(ProjectEntity entity, ProjectUpdateForm form)
    {
        entity.ProjectName = form.ProjectName;
        entity.Description = form.Description;
        entity.StartDate = form.StartDate;
        entity.EndDate = form.EndDate;
        entity.UserId = form.UserId;
        entity.StatusId = form.StatusId;
        entity.CustomerId = form.CustomerId;
        entity.ServiceId = form.ServiceId;
    }

    public static Project Create(ProjectEntity entity) => new()
    {
        Id = entity.Id,
        ProjectName = entity.ProjectName,
        Description = entity.Description,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
        User = entity.User != null ? UserFactory.Create(entity.User) : null!,
        Status = entity.Status != null ? ActivityStatusFactory.Create(entity.Status) : null!,
        Customer = entity.Customer != null ? CustomerFactory.Create(entity.Customer) : null!,
        Service = entity.Service != null ? ServiceFactory.Create(entity.Service) : null!,
    };
}
