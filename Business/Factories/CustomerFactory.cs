using Business.Models;
using Business.Models.Dtos.Create;
using Business.Models.Dtos.Update;
using Data.Entities;

namespace Business.Factories;

internal class CustomerFactory
{
    public static CustomerEntity Create(CustomerCreateForm form) => new()
    {
        CustomerName = form.CustomerName,
    };

    public static CustomerEntity Update(CustomerUpdateForm form) => new()
    {
        Id = form.Id,
        CustomerName = form.CustomerName,
    };

    public static Customer Create(CustomerEntity entity) => new()
    {
        Id = entity.Id,
        CustomerName = entity.CustomerName,
        Projects = entity.Projects.Select(ProjectFactory.Create).ToList()
    };
}
