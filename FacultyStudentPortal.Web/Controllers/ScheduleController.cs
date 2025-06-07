using ClosedXML.Excel;
using FacultyStudentPortal.DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FacultyStudentPortal.Web.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly ISectionRepository _sectionRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IUserRepository _userRepo;
        private readonly IRoomRepository _roomRepo;

        public ScheduleController(
            ISectionRepository sectionRepo,
            ICourseRepository courseRepo,
            IUserRepository userRepo,
            IRoomRepository roomRepo)
        {
            _sectionRepo = sectionRepo;
            _courseRepo = courseRepo;
            _userRepo = userRepo;
            _roomRepo = roomRepo;
        }

        public async Task<IActionResult> ViewSchedule()
        {
            var sections = (await _sectionRepo.GetAllAsync()).ToList();
            var courses = (await _courseRepo.GetAllAsync()).ToDictionary(c => c.CourseId);
            var faculty = (await _userRepo.GetFacultyUsersAsync()).ToDictionary(u => u.UserId);
            var rooms = (await _roomRepo.GetAllAsync()).ToDictionary(r => r.RoomId);

            var viewModel = sections.Select(s => new
            {
                Course = courses[s.CourseId].Code + " - " + courses[s.CourseId].Title,
                Faculty = faculty.ContainsKey(s.FacultyId) ? faculty[s.FacultyId].FullName : "Unknown",
                Room = rooms.ContainsKey(s.RoomId) ? rooms[s.RoomId].RoomNumber : "Unknown",
                s.SectionType,
                s.DayOfWeek,
                StartTime = s.StartTime.ToString(@"hh\:mm"),
                EndTime = s.EndTime.ToString(@"hh\:mm")
            }).OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime);

            return View(viewModel);
        }

        public async Task<IActionResult> ExportExcel()
        {
            var sections = (await _sectionRepo.GetAllAsync()).ToList();
            var courses = (await _courseRepo.GetAllAsync()).ToDictionary(c => c.CourseId);
            var faculty = (await _userRepo.GetFacultyUsersAsync()).ToDictionary(u => u.UserId);
            var rooms = (await _roomRepo.GetAllAsync()).ToDictionary(r => r.RoomId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Schedule");

            worksheet.Cell(1, 1).Value = "Course";
            worksheet.Cell(1, 2).Value = "Faculty";
            worksheet.Cell(1, 3).Value = "Room";
            worksheet.Cell(1, 4).Value = "Type";
            worksheet.Cell(1, 5).Value = "Day";
            worksheet.Cell(1, 6).Value = "Start";
            worksheet.Cell(1, 7).Value = "End";

            int row = 2;
            foreach (var s in sections)
            {
                worksheet.Cell(row, 1).Value = courses[s.CourseId].Code + " - " + courses[s.CourseId].Title;
                worksheet.Cell(row, 2).Value = faculty.ContainsKey(s.FacultyId) ? faculty[s.FacultyId].FullName : "Unknown";
                worksheet.Cell(row, 3).Value = rooms.ContainsKey(s.RoomId) ? rooms[s.RoomId].RoomNumber : "Unknown";
                worksheet.Cell(row, 4).Value = s.SectionType;
                worksheet.Cell(row, 5).Value = s.DayOfWeek;
                worksheet.Cell(row, 6).Value = s.StartTime.ToString(@"hh\:mm");
                worksheet.Cell(row, 7).Value = s.EndTime.ToString(@"hh\:mm");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Schedule.xlsx");
        }
    }
}
