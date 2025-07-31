using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIS_RubricFeedbackGenerator.Models
{
    public class RubricFormViewModel
    {
        [Required(ErrorMessage = "TaskId is required.")]
        public string? TaskId { get; set; }

        public string? TaskTitle { get; set; }

        public string? RubricTitle { get; set; }
        public string? Description { get; set; }

        public List<CriterionInputModel> Criteria { get; set; } = new()
        {
            new CriterionInputModel()
        };

        public List<ScoreDefinitionInputModel> ScoreDefinitions { get; set; } = new()
        {
            new() { ScoreValue = 4, ScoreName = "Excellent" },
            new() { ScoreValue = 3, ScoreName = "Good" },
            new() { ScoreValue = 2, ScoreName = "Fair" },
            new() { ScoreValue = 1, ScoreName = "Poor" },
            new() { ScoreValue = 0, ScoreName = "Very Poor" }
        };

        public List<List<string>> ScoreLevelDescriptions { get; set; } = new()
        {
            new() { "", "", "", "", "" }
        };
    }
}