namespace Texnokaktus.ProgOlymp.Data.Models;

public record Name(string FirstName, string LastName, string? Patronym)
{
    public string FullName => string.Join(" ", new[] { LastName, FirstName, Patronym }.Where(x => x is not null));
}
