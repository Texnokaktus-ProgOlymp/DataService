using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Texnokaktus.ProgOlymp.Admin.DataAccess.Context;

namespace Texnokaktus.ProgOlymp.Admin.DataAccess;

public static class DiUtils
{
    public static IServiceCollection AddDataAccess(this IServiceCollection serviceCollection,
                                                   Action<DbContextOptionsBuilder> optionsAction) =>
        serviceCollection.AddDbContext<AppDbContext>(optionsAction);

    public static IHealthChecksBuilder AddDatabaseHealthChecks(this IHealthChecksBuilder builder) =>
        builder.AddDbContextCheck<AppDbContext>("database");
}
