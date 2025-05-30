using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacultyStudentPortal.Models.Entities
{
    public class Grade
    {
        public int GradeId { get; set; }
        public int AssessmentId { get; set; }
        public int StudentId { get; set; }
        public int Score { get; set; }
        public string Remarks { get; set; }
    }

}
