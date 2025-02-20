using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.Data.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Data.Models;

namespace Texnokaktus.ProgOlymp.Data.Controllers;

public class ContestsController(IRegistrationDataServiceClient client) : Controller
{
    public IActionResult Index() => RedirectToAction(nameof(Contest), new { contestId = 1 });

    [Route("[controller]/{contestId:int}")]
    public async Task<IActionResult> Contest(int contestId) =>
        await GetContestRegistrationsAsync(contestId) is { } contestRegistrations
            ? View(contestRegistrations)
            : NotFound();

    private async Task<ContestRegistrations?> GetContestRegistrationsAsync(int contestId)
    {
        if (await client.GetRegistrationsAsync(contestId) is not { } contestRegistrations)
            return null;

        return new(contestRegistrations.Contest.Name,
                   contestRegistrations.Registrations.Select(registration => registration.MapRegistration()));
    }
}

file static class MappingExtensions
{
    public static Registration MapRegistration(this Common.Contracts.Grpc.Data.Registration registration) =>
        new(registration.Id,
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
