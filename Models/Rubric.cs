using AIS_RubricFeedbackGenerator.Areas.Identity.Data;
using System;
using System.Collections.Generic;

namespace AIS_RubricFeedbackGenerator.Models;

public partial class Rubric
{
    public string RubricId { get; set; } = null!;
    public string TaskId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public double TotalMark { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AIS_RubricFeedbackGeneratorUser? CreatedByNavigation { get; set; }
    public virtual Task? Task { get; set; }
    public virtual ICollection<Criterion> Criteria { get; set; } = new List<Criterion>();
    public ICollection<ScoreDefinition> ScoreDefinitions { get; set; } = new List<ScoreDefinition>();

}
