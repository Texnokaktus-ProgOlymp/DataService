namespace Texnokaktus.ProgOlymp.Data.Models;

public record ParticipantData(Name Name, DateOnly BirthDate, string? Snils, bool IsSnilsValid, string Email, string School, string Region, int Grade);
