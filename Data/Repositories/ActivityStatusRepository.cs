using Data.Contexts;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class ActivityStatusRepository(DataContext context) : BaseRepository<ActivityStatusEntity>(context), IActivityStatusRepository
{
}
