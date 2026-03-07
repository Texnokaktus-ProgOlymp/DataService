namespace Texnokaktus.ProgOlymp.Data.Models;

public record ContestRegistrations(string ContestName, string ContestTitle, IEnumerable<Registration> Registrations);
