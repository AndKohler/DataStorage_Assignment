using Business.Models;
using Business.Models.Dtos.Create;
using Business.Models.Dtos.Update;
using Data.Entities;

namespace Business.Factories;

public static class ServiceFactory
{
    public static ServiceEntity Create(ServiceCreateForm form) => new()
    {
        Name = form.Name,
        Price = form.Price,
    };

    public static ServiceEntity Update(ServiceUpdateForm form) => new()
    {
        Id = form.Id,
        Name = form.Name,
        Price = form.Price,
    };

    public static Service Create(ServiceEntity entity) => new(entity.Id, entity.Name, entity.Price);
}
