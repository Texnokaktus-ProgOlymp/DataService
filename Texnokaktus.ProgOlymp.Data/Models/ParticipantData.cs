namespace Texnokaktus.ProgOlymp.Data.Models;

public record ParticipantData(Name Name,
                              DateOnly BirthDate,
                              string? Snils,
                              bool IsSnilsValid,
                              string Email,
                              string School,
                              string Region,
                              int Grade)
{
    public AgeGroup? AgeGroup => Grade switch
    {
        8 or 9   => new(8, 9),
        10 or 11 => new(10, 11),
        _        => null
    };
};
