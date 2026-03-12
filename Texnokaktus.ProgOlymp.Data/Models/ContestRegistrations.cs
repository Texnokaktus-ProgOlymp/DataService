namespace Texnokaktus.ProgOlymp.Data.Models;

public record ContestRegistrations(string ContestName,
                                   string ContestTitle,
                                   long? PreliminaryYandexContestId,
                                   long? FinalYandexContestId,
                                   IEnumerable<Registration> Registrations);
