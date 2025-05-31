using FacultyStudentPortal.DAL.Interfaces;
using FacultyStudentPortal.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FacultyStudentPortal.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IAssignmentRepository _assignmentRepo;
        private readonly ISubmissionRepository _submissionRepo;
        private readonly IGradeRepository _gradeRepo;
        private readonly IWebHostEnvironment _env;

        public StudentController(
            IAssignmentRepository assignmentRepo,
            ISubmissionRepository submissionRepo,
            IGradeRepository gradeRepo,
            IWebHostEnvironment env)
        {
            _assignmentRepo = assignmentRepo;
            _submissionRepo = submissionRepo;
            _gradeRepo = gradeRepo;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var assignments = await _assignmentRepo.GetByStudentIdAsync(studentId);
            return View(assignments);
        }

        [HttpGet]
        public IActionResult Submit(int assignmentId)
        {
            ViewBag.AssignmentId = assignmentId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int assignmentId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please upload a file.");
                return View();
            }

            int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var path = Path.Combine("/uploads", Guid.NewGuid() + Path.GetExtension(file.FileName));
            using var stream = new FileStream(_env.WebRootPath + path, FileMode.Create);
            await file.CopyToAsync(stream);

            var submission = new Submission
            {
                AssignmentId = assignmentId,
                StudentId = studentId,
                FilePath = path,
                SubmittedAt = DateTime.Now
            };

            await _submissionRepo.AddAsync(submission);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Grades()
        {
            int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var grades = await _gradeRepo.GetByStudentIdAsync(studentId);
            return View(grades);
        }
    }

}
