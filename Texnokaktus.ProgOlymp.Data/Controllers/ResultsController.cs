using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Results;
using Texnokaktus.ProgOlymp.Data.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Data.Models;
using ContestStage = Texnokaktus.ProgOlymp.Data.Models.ContestStage;
using Name = Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Data.Name;
using ProblemResult = Texnokaktus.ProgOlymp.Data.Models.ProblemResult;
using ResultGroup = Texnokaktus.ProgOlymp.Data.Models.ResultGroup;
using ResultRow = Texnokaktus.ProgOlymp.Data.Models.ResultRow;

namespace Texnokaktus.ProgOlymp.Data.Controllers;

[Route("contests/{contestName}/{contestStage}/results")]
public class ResultsController(IRegistrationDataServiceClient registrationDataServiceClient,
                               ResultService.ResultServiceClient resultServiceClient) : Controller
{
    public async Task<IActionResult> ContestStageResults(string contestName, ContestStage contestStage)
    {
        var results = await resultServiceClient.GetResultsAsync(new()
        {
            ContestName = contestName,
            Stage = contestStage switch
            {
                ContestStage.Qualification => Common.Contracts.Grpc.Results.ContestStage.Preliminary,
                ContestStage.Final         => Common.Contracts.Grpc.Results.ContestStage.Final,
                _                          => throw new ArgumentOutOfRangeException(nameof(contestStage), contestStage, null)
            }
        });

        var registrations = await registrationDataServiceClient.GetRegistrationsAsync(contestName) ?? throw new InvalidOperationException("Registrations not found");

        var resultGroups = results.ResultGroups
                                  .Select(group => new ResultGroup(group.Name, group.Rows.Join(registrations.Registrations,
                                                                                               resultRow => resultRow.ParticipantId,
                                                                                               registration => registration.Id,
                                                                                               (resultRow, registration) =>
                                                                                                   new ResultRow()
                                                                                                   {
                                                                                                       Place = resultRow.Place,
                                                                                                       FullName = GetFullName(registration.ParticipantData.Name),
                                                                                                       School = registration.ParticipantData.School,
                                                                                                       Region = registration.ParticipantData.Region,
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

        return View(new ContestStageResult(registrations.Contest.Title, contestStage, resultGroups));
    }

    private static string GetFullName(Name name)
    {
        return string.Join(" ", GetSNameParts(name));

        static IEnumerable<string> GetSNameParts(Name name)
        {
            yield return name.LastName.Trim();
            yield return name.FirstName.Trim();
            if (name.Patronym is not null) yield return name.Patronym.Trim();
        }
    }
}
