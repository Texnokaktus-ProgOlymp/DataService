using Grpc.Core;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Results;
using Texnokaktus.ProgOlymp.Data.Infrastructure.Clients.Abstractions;

namespace Texnokaktus.ProgOlymp.Data.Infrastructure.Clients;

public class ResultServiceClient(ResultService.ResultServiceClient client) : IResultServiceClient
{
    public async Task<ContestResults?> GetContestResultsAsync(string contestName, ContestStage contestStage)
    {
        try
        {
            var request = new GetResultsRequest
            {
                ContestName = contestName,
                Stage = contestStage
            };

            var response = await client.GetResultsAsync(request);
            return response;
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }
}
