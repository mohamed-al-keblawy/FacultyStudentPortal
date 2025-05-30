using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacultyStudentPortal.Models.Entities
{
    public class Assessment
    {
        public int AssessmentId { get; set; }
        public int AssignmentId { get; set; }
        public string Criterion { get; set; }
        public int MaxScore { get; set; }
    }
}
