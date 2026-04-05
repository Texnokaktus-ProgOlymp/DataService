using Texnokaktus.ProgOlymp.Data.Models;

namespace Texnokaktus.ProgOlymp.Data.Extensions;

internal static class MappingExtensions
{
        public static Registration MapRegistration(this Common.Contracts.Grpc.Data.Registration registration) =>
        new(registration.Id,
            new(registration.Uid.ToByteArray()),
            registration.User.MapUser(),
            registration.Created.ToDateTimeOffset(),
            registration.ParticipantData.MapParticipantData(),
            registration.ParentData.MapParentData(),
            registration.TeacherData.MapTeacherData(),
            registration.PersonalDataConsent,
            registration.PreliminaryParticipation?.PapParticipation(),
            registration.FinalParticipation?.PapParticipation());

    private static ContestParticipation PapParticipation(this Common.Contracts.Grpc.Data.ContestParticipation contestParticipation) =>
        new(contestParticipation.YandexContestId,
            contestParticipation.State);

    private static User MapUser(this Common.Contracts.Grpc.Data.User user) =>
        new(user.Id,
            user.Login,
            user.DisplayName,
            user.AvatarId);

    public static ParticipantData MapParticipantData(this Common.Contracts.Grpc.Data.ParticipantData participantData) =>
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
