using Business.Factories;
using Business.Models;
using Business.Models.Dtos.Create;
using Business.Models.Dtos.Update;
using Data.Interfaces;

namespace Business.Services;

/*
- Jag har använt mig utav ChatGPT för många delar under ProjectService då jag haft svårt att göra den.
- Min tanke bakom denna kod är att när jag skapar ett projekt så ska jag även skapa ett customerName, ServiceName och ServicePrice samtidigt.
- Samma om jag ska uppdatera mitt projekt så ska jag även här kunna göra det för CustomerName, ServiceName och ServicePrice.
- Under Delete så vill jag ha en kontroll så att den inte tar bort CustomerName ifall den skulle finnas i ett annat projekt.
- Jag är inte säker på om GetAll funktionen är helt korrekt då jag inte kan använda mig utav .Include av någon anledning som jag 
  inte kan lösa själv.
- User och Status (tex: Ej startad, Pågår, Avslutad) måste redan finnas i databasen för att kunna skapa ett nytt projekt som jag vill välja
  att lägga till när jag skapar ett nytt projekt.
*/

public class ProjectService(
    IProjectRepository projectRepository, 
    IServiceRepository serviceRepository,
    IActivityStatusRepository activityStatusRepository,
    IUserRepository userRepository,
    ICustomerRepository customerRepository
    )

{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IServiceRepository _serviceRepository = serviceRepository;
    private readonly IActivityStatusRepository _activityStatusRepository = activityStatusRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICustomerRepository _customerRepository = customerRepository;

    // CREATE.
    // Jag har lämnat kommentarer för varje moment under Create delen

    public async Task<bool> CreateNewProjectAsync(ProjectCreateForm form)
    {
        if (form == null)
            throw new ArgumentNullException(nameof(form));

        // kollar om det finns en user eller status i databasen. (det måste finnas för att kunna skapa ett nytt projekt)

        bool userExists = await _userRepository.AlreadyExistsAsync(x => x.Id == form.UserId);
        bool statusExists = await _activityStatusRepository.AlreadyExistsAsync(x => x.Id == form.StatusId);

        if (!userExists || !statusExists)
        {
            return false;
        }


        // ========== CUSTOMER HANDLING ================
        // Kollar om det redan finns en customer med samma namn i databasen
        var existingCustomer = await _customerRepository.GetAsync(x => x.CustomerName == form.CustomerName);

        if (existingCustomer != null)
        {
            form.CustomerId = existingCustomer.Id; // Använder Customer om det redan finns.
        }
        else
        {
            // Om det inte finns en Customer med samma namn så skapar man en ny customer med projektet.

            if (string.IsNullOrWhiteSpace(form.CustomerName))
                throw new ArgumentException("Customer name is required to create a new customer.");

            var customerEntity = CustomerFactory.Create(new CustomerCreateForm(form.CustomerName));
            var customerCreationResult = await _customerRepository.CreateAsync(customerEntity);

            if (customerCreationResult == null || customerCreationResult.Id <= 0)
            {
                return false;
            }

            form.CustomerId = customerCreationResult.Id; // Ger nytt CustomerId
        }

        // ========== SERVICE HANDLING ================ 
        // Kollar inte om det redan finns då pris och name(taksname) vill jag skapa med projektet

        if (string.IsNullOrWhiteSpace(form.Name) || form.Price <= 0)
            throw new ArgumentException("Taskname and a valid price are required to create new service.");

        var serviceEntity = ServiceFactory.Create(new ServiceCreateForm(form.Name, form.Price));
        var serviceCreationResult = await _serviceRepository.CreateAsync(serviceEntity);

        if (serviceCreationResult == null || serviceCreationResult.Id <= 0)
            return false;

        form.ServiceId = serviceCreationResult.Id; // Ger nytt ServiceId






        // Skapar Projektet
        var projectEntity = ProjectFactory.Create(form);

        // Sparar Projektet till databasen
        var result = await _projectRepository.CreateAsync(projectEntity);

        return result != null && result.Id > 0; // Retunerar true om Project lyckades att skapas.
    }


    // GET ALL

    public async Task<IEnumerable<Project>> GetAllProjectsAsync()
    {
        var projectEntities = await _projectRepository.GetAllAsync();
        return projectEntities.Select(ProjectFactory.Create);
    }

    // GET BY ID

    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        var projectEntities = await _projectRepository.GetAsync(x => x.Id == id);
        return projectEntities != null ? ProjectFactory.Create(projectEntities) : null!;
    }

    // UPDATE
    // Använt mig utav AI hjälp här med precis som i Create delen. Den gör samma sak här som i Create fast det är update istället.
    public async Task<bool> UpdateProjectAsync(ProjectUpdateForm form)
    {
        var projectEntities = await _projectRepository.GetAsync(x => x.Id == form.Id);

        if (projectEntities != null)
        {
            //========== CUSTOMER HANDLING ===============
            if (!string.IsNullOrWhiteSpace(form.CustomerName))
            {
                // Kollar om CustomerName redan finns.
                var existingCustomer = await _customerRepository.GetAsync(x => x.CustomerName == form.CustomerName);

                if (existingCustomer != null)
                {
                    // Använd Customer on den redan finns.
                    form.CustomerId = existingCustomer.Id;
                }
                else
                {
                    // Om Customer anges så skapas eller uppdateras det.
                    var customerEntity = CustomerFactory.Create(new CustomerCreateForm(form.CustomerName));
                    var customerCreationResult = await _customerRepository.CreateAsync(customerEntity);

                    if (customerCreationResult == null || customerCreationResult.Id <= 0)
                        return false; // Skapa customer failed

                    form.CustomerId = customerCreationResult.Id;
                }
            }
            else
            {
                // Om CustomerName inte anges så behåll den gamla.
                form.CustomerId = projectEntities.CustomerId;
            }

            //=========== SERVICE HANDLING =============
            if (!string.IsNullOrWhiteSpace(form.Name) && form.Price > 0)
            {
                // Om namn och pris anges så skapas eller uppdateras det.
                var serviceEntity = ServiceFactory.Create(new ServiceCreateForm(form.Name, form.Price));
                var serviceCreationResult = await _serviceRepository.CreateAsync(serviceEntity);

                if (serviceCreationResult == null || serviceCreationResult.Id <= 0)
                    return false; // Skapa ny Service failed

                form.ServiceId = serviceCreationResult.Id;
            }
            else
            {
                // Om inget anges så behåll det gamla namnet och priset.
                form.ServiceId = projectEntities.ServiceId;
            }

            //=========== PROJECT UPDATE ============
            ProjectFactory.Update(projectEntities, form);

            // Sparar uppdaterat projekt
            var result = await _projectRepository.UpdateAsync(projectEntities);

            return result != null;
        }

        return false;
    }


    // DELETE
    /* Använt mig utan AI här också. När jag tar bort ett projekt så tar den även bort Service till det project (Name, Price)
     * Sedan kollar den ifall en Customer finns i ett annat projekt och då tar den inte bort customer, (då en customer kanske har flera projekt).
     * Sen till sist så tar den bort projektet.
    */
    public async Task<bool> DeleteProjectAsync(int id)
    {
        var projectEntities = await _projectRepository.GetAsync(x => x.Id == id);
        if (projectEntities != null)
        {
            // ======= SERVICE HANDLING =======
            var service = await _serviceRepository.GetAsync(x => x.Id == projectEntities.ServiceId);
            if (service != null)
            {
                await _serviceRepository.DeleteAsync(service);
            }

            // ========== CUSTOMER HANDLING ================
            var customer = await _customerRepository.GetAsync(x => x.Id == projectEntities.CustomerId);
            if (customer != null)
            {
                bool IfCustomerIsAssociatedWithAnotherProject = await _projectRepository.AlreadyExistsAsync(x => x.CustomerId == customer.Id && x.Id != projectEntities.Id);
                
                if (!IfCustomerIsAssociatedWithAnotherProject)
                {
                    await _customerRepository.DeleteAsync(customer);
                }
            }


            return await _projectRepository.DeleteAsync(projectEntities);
        }

        return false;
    }

}
