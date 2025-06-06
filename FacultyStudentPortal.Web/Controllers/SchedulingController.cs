using FacultyStudentPortal.BLL.Helpers;
using FacultyStudentPortal.BLL.Services;
using FacultyStudentPortal.DAL.Interfaces;
using FacultyStudentPortal.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FacultyStudentPortal.Web.Controllers
{
    public class SchedulingController : Controller
    {
        private readonly ICourseRepository _courseRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly IUserRepository _userRepo;
        private readonly ISectionRepository _sectionRepo;

        public SchedulingController(
            ICourseRepository courseRepo,
            IRoomRepository roomRepo,
            IUserRepository userRepo,
            ISectionRepository sectionRepo)
        {
            _courseRepo = courseRepo;
            _roomRepo = roomRepo;
            _userRepo = userRepo;
            _sectionRepo = sectionRepo;
        }

        public async Task<IActionResult> Generate()
        {
            var courses = (await _courseRepo.GetAllAsync()).ToList();
            var rooms = (await _roomRepo.GetAllAsync()).ToList();
            var faculty = (await _userRepo.GetFacultyUsersAsync()).ToList();

            var lectureSlots = TimeSlotGenerator.GenerateLectureSlots();
            var labSlots = TimeSlotGenerator.GenerateLabSlots();

            var scheduler = new SectionScheduler(courses, faculty, rooms, lectureSlots, labSlots);
            var generatedSections = scheduler.GenerateSchedule();

            foreach (var section in generatedSections)
            {
                await _sectionRepo.AddAsync(section);
            }

            return View("Generated", generatedSections);
        }

    }
}
