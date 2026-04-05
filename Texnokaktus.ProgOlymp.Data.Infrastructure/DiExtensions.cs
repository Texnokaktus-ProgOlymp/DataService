using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Data;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Results;
using Texnokaktus.ProgOlymp.Data.Infrastructure.Clients;
using Texnokaktus.ProgOlymp.Data.Infrastructure.Clients.Abstractions;

namespace Texnokaktus.ProgOlymp.Data.Infrastructure;

public static class DiExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<RegistrationDataService.RegistrationDataServiceClient>(options => options.Address = configuration.GetConnectionStringUri(nameof(RegistrationDataService)));
        services.AddGrpcClient<ResultService.ResultServiceClient>(options => options.Address = configuration.GetConnectionStringUri(nameof(ResultService)));

        return services.AddScoped<IRegistrationDataServiceClient, RegistrationDataServiceClient>();
    }

    private static Uri? GetConnectionStringUri(this IConfiguration configuration, string name) =>
        configuration.GetConnectionString(name) is { } connectionString
            ? new Uri(connectionString)
            : null;
}
