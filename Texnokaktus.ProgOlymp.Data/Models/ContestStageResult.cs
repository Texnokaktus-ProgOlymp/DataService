namespace Texnokaktus.ProgOlymp.Data.Models;

public record ContestStageResult(string Title,
                                 ContestStage ContestStage,
                                 IReadOnlyCollection<Problem> Problems,
                                 IReadOnlyCollection<ResultGroup> ResultGroups);
