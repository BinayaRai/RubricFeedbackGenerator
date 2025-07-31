using AIS_RubricFeedbackGenerator.Areas.Identity.Data;
using System;
using System.Collections.Generic;

namespace AIS_RubricFeedbackGenerator.Models;

public partial class Criterion
{
    public string CriterionId { get; set; } = null!;

    public string RubricId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int? SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Rubric? Rubric { get; set; }

    public virtual ICollection<ScoreLevel> ScoreLevels { get; set; } = new List<ScoreLevel>();
}
