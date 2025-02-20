using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Data.DataAccess.Context;
using Texnokaktus.ProgOlymp.Data.DataAccess.Entities;
using Texnokaktus.ProgOlymp.Data.DataAccess.Repositories.Abstractions;

namespace Texnokaktus.ProgOlymp.Data.DataAccess.Repositories;

public class ContestRepository(AppDbContext context) : IContestRepository
{
    public Task<Contest[]> GetAllAsync() => context.Contests.AsNoTracking().ToArrayAsync();
    public Task<Contest?> GetAsync(int id) =>
        context.Contests
               .AsNoTracking()
               .Include(contest => contest.Applications)
               .ThenInclude(application => application.Region)
               .FirstOrDefaultAsync(contest => contest.Id == id);
}
