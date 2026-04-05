namespace Texnokaktus.ProgOlymp.Data.Models;

public record ResultRow
{
    public required int Place { get; init; }
    public required string FullName { get; init; }
    public required string School { get; init; }
    public required string Region { get; init; }
    public required IReadOnlyCollection<ProblemResult?> Results { get; init; }
    public required decimal? TotalScore { get; init; }
}
