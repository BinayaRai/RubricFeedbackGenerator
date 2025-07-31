using System.ComponentModel.DataAnnotations;
namespace AIS_RubricFeedbackGenerator.Models
{
    public class CriterionInputModel
    {
        [Required(ErrorMessage = "Criterion title is required.")]
        public string Title { get; set; } = string.Empty;
    }
}
