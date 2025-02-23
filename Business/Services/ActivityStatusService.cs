using Business.Factories;
using Business.Models;
using Business.Models.Dtos.Create;
using Business.Models.Dtos.Update;
using Data.Interfaces;

namespace Business.Services;

public class ActivityStatusService(IActivityStatusRepository activityStatusRepository)
{
    private readonly IActivityStatusRepository _activityStatusRepository = activityStatusRepository;

    // CREATE

    public async Task<bool> CreateActivityStatusAsync(ActivityStatusCreateForm form)
    {
        if (!await _activityStatusRepository.AlreadyExistsAsync(x => x.Status == form.Status))
        {
            var entity = ActivityStatusFactory.Create(form);

            entity = await _activityStatusRepository.CreateAsync(entity);
            if (entity != null && entity.Id > 0)
                return true;
        }
        return false;
    }

    // GET ALL

    public async Task<IEnumerable<ActivityStatus>> GetActivityStatusAsync()
    {
        var activityStatusEntities = await _activityStatusRepository.GetAllAsync();
        return activityStatusEntities.Select(ActivityStatusFactory.Create);
    }

    // GET BY ID

    public async Task<ActivityStatus> GetActivityStatusByIdAsync(int id)
    {
        var activityStatusEntities = await _activityStatusRepository.GetAsync(x => x.Id == id);
        return activityStatusEntities != null ? ActivityStatusFactory.Create(activityStatusEntities) : null!;
    }

    // UPDATE

    public async Task<bool> UpdateActivityStatusAsync(ActivityStatusUpdateForm form)
    {
        var entity = await _activityStatusRepository.GetAsync(x => x.Id == form.Id);

        if (entity != null)
        {
            entity = ActivityStatusFactory.Update(form);
            entity = await _activityStatusRepository.UpdateAsync(entity);
            if (entity != null && entity.Id == form.Id)
                return true;
        }

        return false;
    }

    // DELETE

    public async Task<bool> DeleteActivityStatusAsync(int id)
    {
        var entity = await _activityStatusRepository.GetAsync(x => x.Id == id);
        if (entity != null)
        {
            var result = await _activityStatusRepository.DeleteAsync(entity);
            return result;
        }

        return false;
    }

}