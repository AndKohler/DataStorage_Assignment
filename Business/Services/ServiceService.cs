using Business.Factories;
using Business.Models;
using Business.Models.Dtos.Create;
using Business.Models.Dtos.Update;
using Data.Interfaces;

namespace Business.Services;

public class ServiceService(IServiceRepository serviceRepository)
{
    private readonly IServiceRepository _serviceRepository = serviceRepository;


    // CREATE

    public async Task<bool> CreateNewServiceAsync(ServiceCreateForm form)
    {
        if (!await _serviceRepository.AlreadyExistsAsync(x => x.Name == form.Name))
        {
            var serviceEntities = ServiceFactory.Create(form);

            serviceEntities = await _serviceRepository.CreateAsync(serviceEntities);
            if (serviceEntities != null && serviceEntities.Id > 0)
                return true;
        }
        return false;
    }

    // GET ALL

    public async Task<IEnumerable<Service>> GetServiceAsync()
    {
        var serviceEntities = await _serviceRepository.GetAllAsync();
        return serviceEntities.Select(ServiceFactory.Create);
    }

    // GET BY ID

    public async Task<Service> GetServiceByIdAsync(int id)
    {
        var serviceEntities = await _serviceRepository.GetAsync(x => x.Id == id);
        return serviceEntities != null ? ServiceFactory.Create(serviceEntities) : null!;
    }

    // UPDATE

    public async Task<bool> UpdateServiceAsync(ServiceUpdateForm form)
    {
        var serviceEntities = await _serviceRepository.GetAsync(x => x.Id == form.Id);

        if (serviceEntities != null)
        {
            serviceEntities = ServiceFactory.Update(form);
            serviceEntities = await _serviceRepository.UpdateAsync(serviceEntities);
            if (serviceEntities != null && serviceEntities.Id == form.Id)
                return true;
        }

        return false;
    }

    // DELETE

    public async Task<bool> DeleteServiceAsync(int id)
    {
        var serviceEntities = await _serviceRepository.GetAsync(x => x.Id == id);
        if (serviceEntities != null)
        {
            var result = await _serviceRepository.DeleteAsync(serviceEntities);
            return result;
        }

        return false;
    }
}