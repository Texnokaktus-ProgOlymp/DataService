namespace Texnokaktus.ProgOlymp.Data.DataAccess.Entities;

public class ContestStage
{
    public long Id { get; set; }
    public string Name { get; set; }

    public DateTimeOffset ContestStart { get; set; }
    public DateTimeOffset? ContestFinish { get; set; }

    public TimeSpan Duration { get; set; }
}
