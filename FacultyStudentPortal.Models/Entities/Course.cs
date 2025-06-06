using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacultyStudentPortal.Models.Entities
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public int CreditHours { get; set; }
        public int LabHour { get; set; }

        public int EligibleStudents { get; set; } 
    }

}
