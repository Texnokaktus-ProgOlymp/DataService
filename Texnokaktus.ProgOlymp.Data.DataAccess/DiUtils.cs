using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Texnokaktus.ProgOlymp.Data.DataAccess.Context;
using Texnokaktus.ProgOlymp.Data.DataAccess.Repositories;
using Texnokaktus.ProgOlymp.Data.DataAccess.Repositories.Abstractions;

namespace Texnokaktus.ProgOlymp.Data.DataAccess;

public static class DiUtils
{
    public static IServiceCollection AddDataAccess(this IServiceCollection serviceCollection,
                                                   Action<DbContextOptionsBuilder> optionsAction) =>
        serviceCollection.AddDbContext<AppDbContext>(optionsAction)
                         .AddScoped<IContestRepository, ContestRepository>();

    public static IHealthChecksBuilder AddDatabaseHealthChecks(this IHealthChecksBuilder builder) =>
        builder.AddDbContextCheck<AppDbContext>("database");
}
