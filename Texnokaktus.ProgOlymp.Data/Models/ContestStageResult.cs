namespace Texnokaktus.ProgOlymp.Data.Models;

public record ContestStageResult(string Title,
                                 ContestStage ContestStage,
                                 IReadOnlyCollection<ResultGroup> ResultGroups);
