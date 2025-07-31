using System.ComponentModel.DataAnnotations;

namespace AIS_RubricFeedbackGenerator.Models
{
    public class ScoreDefinition
    {
        public string ScoreDefinitionId { get; set; } = null!;
        public string RubricId { get; set; } = null!;
        public double ScoreValue { get; set; }
        public string ScoreName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Rubric Rubric { get; set; } = null!;
        public virtual ICollection<ScoreLevel> ScoreLevels { get; set; } = new List<ScoreLevel>();
    }
}
