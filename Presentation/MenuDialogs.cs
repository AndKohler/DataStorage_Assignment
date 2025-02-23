using Business.Models;
using Business.Models.Dtos.Create;
using Business.Models.Dtos.Update;
using Business.Services;

/*
 * Jag har använt mig mycket av ChatGPT för många delar i denna koden. Dels för att jag har haft problem med att får rätt på allt samt tid.
 * Jag har bara lagt till update och delete för Project och inte någon annan då jag inte har tid att göra klart allt.
 * Jag fungerar inte heller till 100%. Appen startar men jag tror där blir något fel när den ska hitta databasen.
*/

namespace Presentation;

public class MenuDialogs
{
    private readonly ActivityStatusService _activityStatusService;
    private readonly CustomerService _customerService;
    private readonly ProjectService _projectService;
    private readonly ServiceService _serviceService;
    private readonly UserService _userService;

    public MenuDialogs(ActivityStatusService activityStatusService, CustomerService customerService, ProjectService projectService, ServiceService serviceService, UserService userService)
    {
        _activityStatusService = activityStatusService;
        _customerService = customerService;
        _projectService = projectService;
        _serviceService = serviceService;
        _userService = userService;
    }

    public async Task MenuOptions()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("------------Welcome--------");
            Console.WriteLine("1. Create a new project");
            Console.WriteLine("2. Create a new user");
            Console.WriteLine("3. Create a new status");
            Console.WriteLine("4. View all projects");
            Console.WriteLine("5. View all users");
            Console.WriteLine("6. View all statuses");
            Console.WriteLine("7. Update a project");
            Console.WriteLine("8. Delete a project");
            Console.WriteLine("9. Exit");
            Console.Write("Please choose an option (1-9): ");

            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await CreateProjectAsync();
                    break;
                
                case "2":
                    await CreateUserAsync();
                    break;

                case "3":
                    await CreateStatusAsync();
                    break;
                
                case "4":
                    await ViewAllProjectsAsync();
                    break;

                case "5":
                    await ViewAllUsersAsync();
                    break;

                case "6":
                    await ViewAllStatusesAsync();
                    break;


                case "7":
                    await UpdateProjectAsync();
                    break;


                case "8":
                    await DeleteProjectAsync();
                    break;


                case "9":
                    Console.WriteLine("Exiting Program...");
                    return;

                default:
                    Console.WriteLine("Invalid choice, Enter a number between 1 & 9");
                    break;
            }
        }
    }


    private async Task CreateProjectAsync()
    {
        Console.Clear();
        Console.WriteLine("--- Create a new Project ---");

        //=================== PROJECT NAME ====================

        // Project Name (Required)
        Console.Write("Enter Project Name: ");
        string projectName = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(projectName))
        {
            Console.WriteLine("Project name is required.");
            return;
        }

        //=================== DESCRIPTION ====================

        // Description (Optional)
        Console.Write("Enter Description (Optional): ");
        string? description = Console.ReadLine()?.Trim();

        //=================== START DATE ====================

        // Start Date (Required)
        Console.Write("Enter Start Date (yyyy-mm-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
        {
            Console.WriteLine("Invalid Start Date.");
            return;
        }

        //=================== END DATE ====================

        // End Date (Optional)
        Console.Write("Enter End Date (optional): ");
        string? endDateInput = Console.ReadLine();
        DateTime? endDate = null;
        if (!string.IsNullOrEmpty(endDateInput) && DateTime.TryParse(endDateInput, out DateTime parsedEndDate))
        {
            endDate = parsedEndDate;
        }

        //=================== CUSTOMER ====================
        // Select or Create Customer (Required)
        Console.WriteLine("Enter Customer Name: ");
        string customerName = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(customerName))
        {
            Console.WriteLine("Customer Name is required.");
            return;
        }

        // Kollar om CustomerName redan finns
        var customers = await _customerService.GetCustomersAsync();
        var existingCustomer = customers?.FirstOrDefault(x => x.CustomerName.Equals(customerName, StringComparison.OrdinalIgnoreCase));

        int customerId;

        if (existingCustomer != null)
        {
            // Om det finns använd deras Id
            customerId = existingCustomer.Id;
            Console.WriteLine($"Customer '{customerName}' already exists. Using existing customer.");
        }
        else
        {
            // Om det inte finns, skapa ny Customer
            Console.WriteLine($"Customer '{customerName}' does not exist. Creating a new customer...");
            var customerCreateForm = new CustomerCreateForm(customerName);
            bool customerCreated = await _customerService.CreateCustomerAsync(customerCreateForm);

            if (customerCreated)
            {
                // Får ett nytt customerId
                var newCustomer = await _customerService.GetCustomersAsync();
                customerId = newCustomer?.First(c => c.CustomerName.Equals(customerName, StringComparison.OrdinalIgnoreCase)).Id ?? 0;
                Console.WriteLine($"New customer '{customerName}' created successfully.");
            }
            else
            {
                Console.WriteLine("Failed to create customer.");
                return;
            }
        }

        //=================== SERVICE ====================
        Console.Write("Enter Service Name: ");
        string serviceName = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(serviceName))
        {
            Console.WriteLine("Service Name is required.");
            return;
        }

        Console.Write("Enter Service Price: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal servicePrice) || servicePrice <= 0)
        {
            Console.WriteLine("Invalid Service Price.");
            return;
        }

        var serviceCreateForm = new ServiceCreateForm(serviceName, servicePrice);
        bool serviceCreated = await _serviceService.CreateNewServiceAsync(serviceCreateForm);

        if (!serviceCreated)
        {
            Console.WriteLine("Failed to create service.");
            return;
        }

        var services = await _serviceService.GetServiceAsync();
        var newService = services?.FirstOrDefault(x => x.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase) && x.Price == servicePrice);

        if (newService == null)
        {
            Console.WriteLine("Failed to retrieve newly created service.");
            return;
        }

        int serviceId = newService.Id;

        //=================== USER ====================

        // Select User (Manager for the Project) (Required)
        Console.WriteLine("Select a User for the Project: ");
        var users = await _userService.GetUsersAsync();
        if (users == null || !users.Any())
        {
            Console.WriteLine("No users available.");
            return;
        }
        for (int i = 0; i < users.Count(); i++)
        {
            var user = users.ElementAt(i);
            Console.WriteLine($"{i + 1}. {user.FirstName} {user.LastName}");
        }
        Console.WriteLine("Enter user number to select: ");
        int userChoice;
        if (!int.TryParse(Console.ReadLine(),out userChoice) || userChoice <= 0 || userChoice > users.Count())
        {
            Console.WriteLine("Invalid selection.");
            return;
        }
        var selectedUser = users.ElementAt(userChoice - 1);

        //=================== STATUS ====================

        // Select Status (Started, On-Going, Finished) (Required)
        Console.WriteLine("Select a Status for the Project");
        var statuses = await _activityStatusService.GetActivityStatusAsync();
        if (statuses == null || !statuses.Any())
        {
            Console.WriteLine("No statuses available.");
            return;
        }
        for (int i = 0; i < statuses.Count(); i++)
        {
            var status = statuses.ElementAt(i);
            Console.WriteLine($"{i + 1}. {status.Status}");
        }
        Console.WriteLine("Enter Status number to select: ");
        int statusChoice;
        if (!int.TryParse(Console.ReadLine(), out statusChoice) || statusChoice <= 0 || statusChoice > statuses.Count())
        {
            Console.WriteLine("Invalid Selection.");
            return;
        }
        var selectedStatus = statuses.ElementAt(statusChoice - 1);

        var projectCreateForm = new ProjectCreateForm
        {
            ProjectName = projectName,
            Description = description,
            StartDate = startDate,
            EndDate = endDate,
            UserId = selectedUser.Id,
            StatusId = selectedStatus.Id,
            CustomerId = customerId,
            ServiceId = serviceId,
            CustomerName = customerName,
            Name = serviceName,
            Price = servicePrice
        };

        bool projectCreated = await _projectService.CreateNewProjectAsync(projectCreateForm);
        if (projectCreated)
        {
            Console.WriteLine("Project created successfully!");
        }
        else
        {
            Console.WriteLine("Failed to create project.");
        }
    }

    private async Task CreateUserAsync()
    {
        Console.Clear();
        Console.WriteLine("--- Create a New User ---");

        // First Name (Required)
        Console.Write("Enter First Name: ");
        string firstName = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(firstName))
        {
            Console.WriteLine("First Name is required.");
            return;
        }

        // Last Name (Required)
        Console.Write("Enter Last Name: ");
        string lastName = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(lastName))
        {
            Console.WriteLine("Last Name is required.");
            return;
        }

        // Email (Required)
        Console.Write("Enter Email: ");
        string email = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email is required.");
            return;
        }

        // PhoneNumber (Required)
        Console.Write("Enter PhoneNumber: ");
        string phoneNumber = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            Console.WriteLine("PhoneNumber is required.");
            return;
        }

        // Create user via UserService
        var userCreateForm = new UserCreateForm(firstName, lastName, email, phoneNumber);
        
        bool userCreated = await _userService.CreateNewUserAsync(userCreateForm);
        if (userCreated)
        {
            Console.WriteLine("User created successfully!");
        }
        else
        {
            Console.WriteLine("Failed to create user.");
        }

    }

    private async Task CreateStatusAsync()
    {
        Console.Clear();
        Console.WriteLine("--- Create a New Status ---");

        // Status (Required)
        Console.Write("Enter Status (e.g., Started, On-Going, Finished): ");
        string status = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(status))
        {
            Console.WriteLine("Status is required.");
            return;
        }

        // Create status via ActivityStatusService
        var statusCreateForm = new ActivityStatusCreateForm(status);

        bool statusCreated = await _activityStatusService.CreateActivityStatusAsync(statusCreateForm);
        if (statusCreated)
        {
            Console.WriteLine("Status created successfully!");
        }
        else
        {
            Console.WriteLine("Failed to create status.");
        }

    }

    private async Task ViewAllProjectsAsync()
    {
        Console.Clear();
        Console.WriteLine("--- All Projects ---");

        var projects = await _projectService.GetAllProjectsAsync();
        if (projects == null || !projects.Any())
        {
            Console.WriteLine("No projects available.");
            return;
        }

        foreach (var project in projects)
        {
            Console.WriteLine($"Project Name: {project.ProjectName}, Description: {project.Description}, StartDate: {project.StartDate}, EndDate: {project.EndDate}, Manager:");
        }
        Console.ReadKey();
    }

    private async Task ViewAllUsersAsync()
    {
        Console.Clear();
        Console.WriteLine("--- All Users ---");

        var users = await _userService.GetUsersAsync();
        if (users == null || !users.Any())
        {
            Console.WriteLine("No users available.");
            return;
        }

        foreach (var user in users)
        {
            Console.WriteLine($"Name: {user.FirstName} {user.LastName}, Email: {user.Email}, PhoneNumber: {user.PhoneNumber}");
        }
        Console.ReadKey();
    }

    private async Task ViewAllStatusesAsync()
    {
        Console.Clear();
        Console.WriteLine("--- All Statuses ---");

        var statuses = await _activityStatusService.GetActivityStatusAsync();
        if (statuses == null || !statuses.Any())
        {
            Console.WriteLine("No statuses available.");
            return;
        }

        foreach (var status in statuses)
        {
            Console.WriteLine($"Status: {status.Status}");
        }
        Console.ReadKey();
    }

    private async Task UpdateProjectAsync()
    {
        Console.Clear();
        Console.WriteLine("--- Update an Existing Project ---");

        // Fetch all projects
        var projects = await _projectService.GetAllProjectsAsync();
        if (projects == null || !projects.Any())
        {
            Console.WriteLine("No projects available to update.");
            return;
        }

        // Display projects for selection
        for (int i = 0; i < projects.Count(); i++)
        {
            var project = projects.ElementAt(i);
            Console.WriteLine($"{i + 1}. {project.ProjectName} (ID: {project.Id})");
        }

        Console.Write("Enter project number to update: ");
        if (!int.TryParse(Console.ReadLine(), out int projectChoice) || projectChoice <= 0 || projectChoice > projects.Count())
        {
            Console.WriteLine("Invalid selection.");
            return;
        }

        var selectedProject = projects.ElementAt(projectChoice - 1);

        // Ask for new details (if left blank, keep existing values)
        Console.Write($"Enter new Project Name ({selectedProject.ProjectName}): ");
        string newProjectName = Console.ReadLine()?.Trim();
        newProjectName = string.IsNullOrWhiteSpace(newProjectName) ? selectedProject.ProjectName : newProjectName;

        Console.Write($"Enter new Description ({selectedProject.Description ?? "None"}): ");
        string? newDescription = Console.ReadLine()?.Trim();
        newDescription = string.IsNullOrWhiteSpace(newDescription) ? selectedProject.Description : newDescription;

        Console.Write($"Enter new Start Date ({selectedProject.StartDate:yyyy-MM-dd}): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime newStartDate))
        {
            newStartDate = selectedProject.StartDate;
        }

        Console.Write($"Enter new End Date ({selectedProject.EndDate?.ToString("yyyy-MM-dd") ?? "None"}): ");
        string? endDateInput = Console.ReadLine();
        DateTime? newEndDate = string.IsNullOrEmpty(endDateInput) ? selectedProject.EndDate : DateTime.Parse(endDateInput);

        // Create update form and send request
        var updateForm = new ProjectUpdateForm
        {
            Id = selectedProject.Id,
            ProjectName = newProjectName,
            Description = newDescription,
            StartDate = newStartDate,
            EndDate = newEndDate,
            UserId = selectedProject.Id,
            StatusId = selectedProject.Id,
            CustomerId = selectedProject.Id,
            ServiceId = selectedProject.Id
        };

        bool updated = await _projectService.UpdateProjectAsync(updateForm);
        Console.WriteLine(updated ? "Project updated successfully!" : "Failed to update project.");
    }

    private async Task DeleteProjectAsync()
    {
        Console.Clear();
        Console.WriteLine("--- Delete a Project ---");

        var projects = await _projectService.GetAllProjectsAsync();
        if (projects == null || !projects.Any())
        {
            Console.WriteLine("No projects available to delete.");
            return;
        }

        for (int i = 0; i < projects.Count(); i++)
        {
            var project = projects.ElementAt(i);
            Console.WriteLine($"{i + 1}. {project.ProjectName} (ID: {project.Id})");
        }

        Console.Write("Enter the number of the project you want to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int projectChoice) || projectChoice <= 0 || projectChoice > projects.Count())
        {
            Console.WriteLine("Invalid selection.");
            return;
        }

        var selectedProject = projects.ElementAt(projectChoice - 1);

        // Confirm deletion
        Console.Write($"Are you sure you want to delete '{selectedProject.ProjectName}'? (yes/no): ");
        string confirmation = Console.ReadLine()?.Trim().ToLower();
        if (confirmation != "yes")
        {
            Console.WriteLine("Project deletion canceled.");
            return;
        }

        bool deleted = await _projectService.DeleteProjectAsync(selectedProject.Id);
        Console.WriteLine(deleted ? "Project deleted successfully!" : "Failed to delete project.");
    }
}
