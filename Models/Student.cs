using AIS_RubricFeedbackGenerator.Areas.Identity.Data;
using System;
using System.Collections.Generic;

namespace AIS_RubricFeedbackGenerator.Models;

public partial class Student
{
    public string StudentId { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual AIS_RubricFeedbackGeneratorUser? CreatedByNavigation { get; set; }
}
