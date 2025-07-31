using System.ComponentModel.DataAnnotations;

namespace AIS_RubricFeedbackGenerator.Models
{
    public class StudentViewModel
    {
        [Required(ErrorMessage = "Student ID is required")]
        [StringLength(20, ErrorMessage = "Student ID cannot be longer than 20 characters")]
        [Display(Name = "Student ID")]
        public string StudentId { get; set; } = null!;

        [StringLength(50, ErrorMessage = "Full name cannot be longer than 50 characters")]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(50, ErrorMessage = "Email cannot be longer than 50 characters")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        // Convert to Student entity
        public Student ToStudent()
        {
            return new Student
            {
                StudentId = this.StudentId,
                FullName = this.FullName,
                Email = this.Email,
                CreatedBy = this.CreatedBy,
                CreatedAt = this.CreatedAt
            };
        }

        // Create from Student entity
        public static StudentViewModel FromStudent(Student student)
        {
            return new StudentViewModel
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                CreatedBy = student.CreatedBy,
                CreatedAt = student.CreatedAt
            };
        }
    }
}