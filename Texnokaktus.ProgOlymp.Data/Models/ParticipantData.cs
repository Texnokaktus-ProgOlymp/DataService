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
    public string AgeGroup => Grade switch
    {
        8 or 9   => "8-9 класс",
        10 or 11 => "10-11 класс",
        _        => "-"
    };
};
