using Business.Factories;
using Business.Models;
using Business.Models.Dtos.Create;
using Business.Models.Dtos.Update;
using Data.Interfaces;

namespace Business.Services;

public class UserService(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    // CREATE

    public async Task<bool> CreateNewUserAsync(UserCreateForm form)
    {
        if (!await _userRepository.AlreadyExistsAsync(x => x.Email == form.Email))
        {
            var userEntities = UserFactory.Create(form);

            userEntities = await _userRepository.CreateAsync(userEntities);
            if (userEntities != null && userEntities.Id > 0)
                return true;
        }
        return false;
    }

    // GET ALL

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        var userEntities = await _userRepository.GetAllAsync();
        return userEntities.Select(UserFactory.Create);
    }

    // GET USER BY ID

    public async Task<User> GetUserByIdAsync(int id)
    {
        var userEntities = await _userRepository.GetAsync(x => x.Id == id);
        return userEntities != null ? UserFactory.Create(userEntities) : null!;
    }

    // GET USER BY EMAIL

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var userEntities = await _userRepository.GetAsync(x => x.Email == email);
        return userEntities != null ? UserFactory.Create(userEntities) : null!;
    }

    // UPDATE

    public async Task<bool> UpdateUserAsync(UserUpdateForm form)
    {
        var userEntities = await _userRepository.GetAsync(x => x.Id == form.Id);

        if (userEntities != null)
        {
            userEntities = UserFactory.Update(form);
            userEntities = await _userRepository.UpdateAsync(userEntities);
            if (userEntities != null && userEntities.Id == form.Id)
                return true;
        }

        return false;
    }

    // DELETE

    public async Task<bool> DeleteUserAsync(int id)
    {
        var userEntities = await _userRepository.GetAsync(x => x.Id == id);
        if (userEntities != null)
        {
            var result = await _userRepository.DeleteAsync(userEntities);
            return result;
        }

        return false;
    }
}
