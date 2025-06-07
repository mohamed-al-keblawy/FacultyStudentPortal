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
    }
}
