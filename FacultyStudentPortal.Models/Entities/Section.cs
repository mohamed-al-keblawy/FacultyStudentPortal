using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacultyStudentPortal.Models.Entities
{
    public class Section
    {
        public int SectionId { get; set; }
        public int CourseId { get; set; }
        public int FacultyId { get; set; }
        public int RoomId { get; set; }
        public string DayOfWeek { get; set; } // e.g. "Monday"
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

}
