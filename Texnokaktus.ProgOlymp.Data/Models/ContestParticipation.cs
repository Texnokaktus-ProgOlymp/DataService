using Texnokaktus.ProgOlymp.Common.Contracts.Grpc.Common;

namespace Texnokaktus.ProgOlymp.Data.Models;

public record ContestParticipation(long YandexContestParticipantId, ParticipationState ParticipationState);
