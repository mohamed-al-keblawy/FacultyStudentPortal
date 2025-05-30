using System.ComponentModel.DataAnnotations;

namespace FacultyStudentPortal.Web.ViewModels
{
    public class AssessmentItem
    { 
        [Required]
        public string Criterion { get; set; }

        [Required]
        [Range(1,100)]
        public int MaxScore { get; set; }
    }
    public class CreateAssessmentViewModel
    {
        [Required]
        public int AssignmentId { get; set; }
        [Required]
        public List<AssessmentItem> Criteria { get; set; } = new List<AssessmentItem>();
        }
}
