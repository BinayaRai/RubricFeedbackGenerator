using AIS_RubricFeedbackGenerator.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIS_RubricFeedbackGenerator.ViewModels
{
    public class TaskFormViewModel
    {
        public string TaskId { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please complete the details.")]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<Rubric> Rubrics { get; set; } = new List<Rubric>();
    }
}
