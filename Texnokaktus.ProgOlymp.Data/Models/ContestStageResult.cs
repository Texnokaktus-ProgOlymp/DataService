namespace Texnokaktus.ProgOlymp.Data.Models;

public record ContestStageResult(string Name,
                                 string Title,
                                 ContestStage ContestStage,
                                 IReadOnlyCollection<Problem> Problems,
                                 IReadOnlyCollection<ResultGroup> ResultGroups);
