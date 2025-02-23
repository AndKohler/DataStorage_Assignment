using Business.Models;
using Business.Models.Dtos.Create;
using Business.Models.Dtos.Update;
using Data.Entities;

namespace Business.Factories;

public static class ActivityStatusFactory
{
    public static ActivityStatusEntity Create(ActivityStatusCreateForm form) => new()
    {
        Status = form.Status
    };

    public static ActivityStatusEntity Update(ActivityStatusUpdateForm form) => new()
    {
        Id = form.Id,
        Status = form.Status
    };

    public static ActivityStatus Create(ActivityStatusEntity entity) => new(entity.Id, entity.Status);
}
