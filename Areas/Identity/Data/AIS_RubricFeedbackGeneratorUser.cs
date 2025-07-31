using AIS_RubricFeedbackGenerator.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIS_RubricFeedbackGenerator.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class AIS_RubricFeedbackGeneratorUser : IdentityUser
{
    public ICollection<Rubric> Rubrics { get; set; } = new List<Rubric>();
    public ICollection<Student> Students { get; set; } = new List<Student>();
}

