using Business.Services;
using Data.Contexts;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation;

var services = new ServiceCollection()
    .AddDbContext<DataContext>(options => options.UseSqlServer(@"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Workspace\\DataStorage_Assignment\\Data\\Databases\\database.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=True"))
    .AddScoped<IActivityStatusRepository, ActivityStatusRepository>()
    .AddScoped<ICustomerRepository, CustomerRepository>()
    .AddScoped<IProjectRepository, ProjectRepository>()
    .AddScoped<IServiceRepository, ServiceRepository>()
    .AddScoped<IUserRepository, UserRepository>()
    .AddScoped<ActivityStatusService>()
    .AddScoped<CustomerService>()
    .AddScoped<ProjectService>()
    .AddScoped<ServiceService>()
    .AddScoped<UserService>()
    .AddScoped<MenuDialogs>()
    .BuildServiceProvider();

var menuDialogs = services.GetRequiredService<MenuDialogs>();
await menuDialogs.MenuOptions();