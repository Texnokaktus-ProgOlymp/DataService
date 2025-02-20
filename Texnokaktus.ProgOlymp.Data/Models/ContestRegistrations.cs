namespace Texnokaktus.ProgOlymp.Data.Models;

public record ContestRegistrations(int ContestId, string ContestName, IEnumerable<Registration> Registrations);
