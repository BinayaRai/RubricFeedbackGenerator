namespace AIS_RubricFeedbackGenerator.Models;

public partial class ScoreLevel
{
    public string ScoreLevelId { get; set; } = null!;
    public string CriterionId { get; set; } = null!;
    public string ScoreDefinitionId { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Criterion? Criterion { get; set; }
    public virtual ScoreDefinition? ScoreDefinition { get; set; }
}
