namespace Texnokaktus.ProgOlymp.Data.Models;

public record ResultRow
{
    public required int Place { get; init; }
    public required ParticipantData Participant { get; init; }
    public required IReadOnlyCollection<ProblemResult?> Results { get; init; }
    public required decimal? TotalScore { get; init; }
}
