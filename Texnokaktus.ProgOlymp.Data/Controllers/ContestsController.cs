using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Texnokaktus.ProgOlymp.Data.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Data.Models;
using Texnokaktus.ProgOlymp.Data.Options;
using Texnokaktus.ProgOlymp.Data.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Data.Controllers;

public class ContestsController(IRegistrationDataServiceClient client, IExcelService excelService, IOptions<ContestRoutingOptions> routingOptions) : Controller
{
    public IActionResult Index() =>
        RedirectToAction(nameof(Contest),
                         new
                         {
                             contestId = routingOptions.Value.DefaultContest
                         });

    [Route("[controller]/{contestName}")]
    public async Task<IActionResult> Contest(string contestName) =>
        await GetContestRegistrationsAsync(contestName) is { } contestRegistrations
            ? View(contestRegistrations)
            : RedirectToAction(nameof(Index));

    [Route("[controller]/{contestName}/excel")]
    public async Task<IActionResult> Excel(string contestName)
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
                   contestRegistrations.Contest.Name,
                   contestRegistrations.Registrations.Select(registration => registration.MapRegistration()));
    }
}

file static class MappingExtensions
{
    public static Registration MapRegistration(this Common.Contracts.Grpc.Data.Registration registration) =>
        new(registration.Id,
            new(registration.Uid.ToByteArray()),
            registration.User.MapUser(),
            registration.Created.ToDateTimeOffset(),
            registration.ParticipantData.MapParticipantData(),
            registration.ParentData.MapParentData(),
            registration.TeacherData.MapTeacherData(),
            registration.PersonalDataConsent);

    private static User MapUser(this Common.Contracts.Grpc.Data.User user) =>
        new(user.Id,
            user.Login,
            user.DisplayName,
            user.AvatarId);

    private static ParticipantData MapParticipantData(this Common.Contracts.Grpc.Data.ParticipantData participantData) =>
        new(participantData.Name.MapName(),
            DateOnly.FromDateTime(participantData.BirthDate.ToDateTime().ToUniversalTime()),
            participantData.Snils,
            participantData.IsSnilsValid,
            participantData.Email,
            participantData.School,
            participantData.Region,
            participantData.Grade);

    private static ParentData MapParentData(this Common.Contracts.Grpc.Data.ParentData parentData) =>
        new(parentData.Name.MapName(),
            parentData.Email,
            parentData.Phone);

    private static TeacherData MapTeacherData(this Common.Contracts.Grpc.Data.TeacherData parentData) =>
        new(parentData.Name.MapName(),
            parentData.Email,
            parentData.Phone,
            parentData.School);

    private static Name MapName(this Common.Contracts.Grpc.Data.Name name) =>
        new(name.FirstName, name.LastName, name.Patronym);
}
