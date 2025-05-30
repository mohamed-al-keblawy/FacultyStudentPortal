using System.ComponentModel.DataAnnotations;

namespace FacultyStudentPortal.Web.ViewModels
{
    public class CreateAssignmentViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public IFormFile File { get; set; }
    }
}
