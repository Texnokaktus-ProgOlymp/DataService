using Texnokaktus.ProgOlymp.Data.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.Data.DataAccess.Repositories.Abstractions;

public interface IContestRepository
{
    Task<Contest[]> GetAllAsync();
    Task<Contest?> GetAsync(int id);
}
