using System.ComponentModel.DataAnnotations;
namespace AIS_RubricFeedbackGenerator.Models
{
    public class ScoreDefinitionInputModel
    {
        public double ScoreValue { get; set; }
        public string ScoreName { get; set; } = string.Empty;
    }
}
