using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Texnokaktus.ProgOlymp.Data.Extensions;
using Texnokaktus.ProgOlymp.Data.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Data.Models;
using Texnokaktus.ProgOlymp.Data.Options;
using Texnokaktus.ProgOlymp.Data.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Data.Controllers;

public class ContestsController(IRegistrationDataServiceClient client) : Controller
{
    public IActionResult Index([FromServices] IOptions<ContestRoutingOptions> routingOptions) =>
        RedirectToAction(nameof(Contest),
                         new
                         {
                             contestName = routingOptions.Value.DefaultContest
                         });

    [Route("[controller]/{contestName}")]
    public async Task<IActionResult> Contest(string contestName) =>
        await GetContestRegistrationsAsync(contestName) is { } contestRegistrations
            ? View(contestRegistrations)
            : RedirectToAction(nameof(Index));

    [Route("[controller]/{contestName}/excel")]
    public async Task<IActionResult> Excel(string contestName, [FromServices] IExcelService excelService)
    {
        if (await GetContestRegistrationsAsync(contestName) is not { } contestRegistrations)
            return NotFound();

        var stream = excelService.GenerateExcel(contestRegistrations.Registrations);

        return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"registrations-{DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(3)):s}.xlsx");
    }

    private async Task<ContestRegistrations?> GetContestRegistrationsAsync(string contestName)
    {
        if (await client.GetRegistrationsAsync(contestName) is not { } contestRegistrations)
            return null;

        return new(contestName,
                   contestRegistrations.Contest.Title,
                   contestRegistrations.Contest.PreliminaryStageContestId,
                   contestRegistrations.Contest.FinalStageContestId,
                   contestRegistrations.Registrations.Select(registration => registration.MapRegistration()));
    }
}
