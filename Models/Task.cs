using AIS_RubricFeedbackGenerator.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace AIS_RubricFeedbackGenerator.Models
{
    public class Task
    {
        [BindNever]
        public string? TaskId { get; set; }
        [Required(ErrorMessage = "Please complete the details.")]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public double TotalMark { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual AIS_RubricFeedbackGeneratorUser? CreatedByNavigation { get; set; }
        public virtual ICollection<Rubric> Rubrics { get; set; } = new List<Rubric>();
    }
}
