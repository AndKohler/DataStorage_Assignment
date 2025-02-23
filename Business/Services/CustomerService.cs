using Business.Factories;
using Business.Models;
using Business.Models.Dtos.Create;
using Business.Models.Dtos.Update;
using Data.Interfaces;

namespace Business.Services;

public class CustomerService(ICustomerRepository customerRepository)
{
    private readonly ICustomerRepository _customerRepository = customerRepository;

    // CREATE

    public async Task<bool> CreateCustomerAsync(CustomerCreateForm form)
    {
        if (!await _customerRepository.AlreadyExistsAsync(x => x.CustomerName == form.CustomerName))
        {
            var entity = CustomerFactory.Create(form);

            entity = await _customerRepository.CreateAsync(entity);
            if (entity != null && entity.Id > 0)
                return true;
        }
        return false;
    }

    // GET ALL

    public async Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        var customerEntities = await _customerRepository.GetAllAsync();
        return customerEntities.Select(CustomerFactory.Create);
    }

    // GET BY ID

    public async Task<Customer> GetCustomerByIdAsync(int id)
    {
        var customerEntities = await _customerRepository.GetAsync(x => x.Id == id);
        return customerEntities != null ? CustomerFactory.Create(customerEntities) : null!;
    }

    // GET BY NAME

    public async Task<Customer> GetCustomerByCustomerNameAsync(string customerName)
    {
        var customerEntities = await _customerRepository.GetAsync(x => x.CustomerName == customerName);
        return customerEntities != null ? CustomerFactory.Create(customerEntities) : null!;
    }

    // UPDATE

    public async Task<bool> UpdateCustomerAsync(CustomerUpdateForm form)
    {
        var entity = await _customerRepository.GetAsync(x => x.Id == form.Id);
        
        if (entity != null)
        {
            entity = CustomerFactory.Update(form);
            entity = await _customerRepository.UpdateAsync(entity);
            if (entity != null && entity.Id == form.Id)
                return true;
        }

        return false;
    }

    // DELETE

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        var entity = await _customerRepository.GetAsync(x => x.Id == id);
        if (entity != null)
        {
            var result = await _customerRepository.DeleteAsync(entity);
            return result;
        }

        return false;
    }
}
