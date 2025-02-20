using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Data;

namespace Texnokaktus.ProgOlymp.Data.Infrastructure.Clients.Abstractions;

public interface IRegistrationDataServiceClient
{
    Task<ContestRegistrations?> GetRegistrationsAsync(int contestId);
}
