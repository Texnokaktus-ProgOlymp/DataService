using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Results;

namespace Texnokaktus.ProgOlymp.Data.Infrastructure.Clients.Abstractions;

public interface IResultServiceClient
{
    Task<ContestResults?> GetContestResultsAsync(string contestName, ContestStage contestStage);
}
