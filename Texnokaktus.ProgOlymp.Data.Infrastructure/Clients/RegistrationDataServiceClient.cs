using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Data;
using Texnokaktus.ProgOlymp.Data.Infrastructure.Clients.Abstractions;

namespace Texnokaktus.ProgOlymp.Data.Infrastructure.Clients;

public class RegistrationDataServiceClient(RegistrationDataService.RegistrationDataServiceClient client) : IRegistrationDataServiceClient
{
    public async Task<ContestRegistrations?> GetRegistrationsAsync(int contestId)
    {
        try
        {
            var request = new GetRegistrationsRequest
            {
                ContestId = contestId
            };

            var response = await client.GetRegistrationsAsync(request);
            return response.Result;
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }
}
