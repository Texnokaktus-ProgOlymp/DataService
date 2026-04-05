using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.Data.Extensions;
using Texnokaktus.ProgOlymp.Data.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Data.Models;
using ContestStage = Texnokaktus.ProgOlymp.Data.Models.ContestStage;
using Problem = Texnokaktus.ProgOlymp.Data.Models.Problem;
using ProblemResult = Texnokaktus.ProgOlymp.Data.Models.ProblemResult;
using ResultGroup = Texnokaktus.ProgOlymp.Data.Models.ResultGroup;
using ResultRow = Texnokaktus.ProgOlymp.Data.Models.ResultRow;

namespace Texnokaktus.ProgOlymp.Data.Controllers;

[Route("contests/{contestName}/{contestStage}/results")]
public class ResultsController(IRegistrationDataServiceClient registrationDataServiceClient,
                               IResultServiceClient resultServiceClient) : Controller
{
    public async Task<IActionResult> ContestStageResults(string contestName, ContestStage contestStage)
    {
        var results = await resultServiceClient.GetContestResultsAsync(contestName,
                                                                       contestStage switch
                                                                       {
                                                                           ContestStage.Qualification => Common.Contracts.Grpc.Results.ContestStage.Preliminary,
                                                                           ContestStage.Final         => Common.Contracts.Grpc.Results.ContestStage.Final,
                                                                           _                          => throw new ArgumentOutOfRangeException(nameof(contestStage), contestStage, null)
                                                                       })
                      ?? throw new InvalidOperationException("Results not found");

        var registrations = await registrationDataServiceClient.GetRegistrationsAsync(contestName)
                         ?? throw new InvalidOperationException("Registrations not found");

        var problems = results.Problems.Select(problem => new Problem(problem.Alias, problem.Name)).ToArray();

        var resultGroups = results.ResultGroups
                                  .Select(group => new ResultGroup(group.Name, group.Rows.Join(registrations.Registrations,
                                                                                               resultRow => resultRow.ParticipantId,
                                                                                               registration => registration.Id,
                                                                                               (resultRow, registration) =>
                                                                                                   new ResultRow
                                                                                                   {
                                                                                                       Place = resultRow.Place,
                                                                                                       Participant = registration.ParticipantData.MapParticipantData(),
                                                                                                       Results = resultRow.Results
                                                                                                                          .Select(problemResult => problemResult?.Score is { } score
                                                                                                                                                       ? new ProblemResult(score.BaseScore,
                                                                                                                                                                           score.AdjustmentsSum)
                                                                                                                                                       : (ProblemResult?)null)
                                                                                                                          .ToArray(),
                                                                                                       TotalScore = resultRow.TotalScore
                                                                                                   }
                                                                                              ).ToArray()))
                                  .ToArray();

        return View(new ContestStageResult(registrations.Contest.Title, contestStage, problems, resultGroups));
    }
}
