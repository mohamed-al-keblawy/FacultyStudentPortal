using FacultyStudentPortal.DAL.Interfaces;
using FacultyStudentPortal.Models.Entities;
using FacultyStudentPortal.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

//[Authorize(Roles = "Faculty")]
public class FacultyController : Controller
{
    private readonly IAssignmentRepository _assignmentRepo;
    private readonly IAssessmentRepository _assessmentRepo;
    private readonly IWebHostEnvironment _env;
    private readonly IUserRepository _userRepo;
    private readonly ISubmissionRepository _submissionRepo;

    public FacultyController(
        IAssignmentRepository assignmentRepo,
        IAssessmentRepository assessmentRepo,
        IWebHostEnvironment env,
        IUserRepository userRepo,
        ISubmissionRepository submissionRepo)
    {
        _assignmentRepo = assignmentRepo;
        _assessmentRepo = assessmentRepo;
        _env = env;
        _userRepo = userRepo;
        _submissionRepo = submissionRepo;
    }

    public IActionResult Index()
    {
        return View(); // Placeholder for dashboard
    }

    [HttpGet]
    public IActionResult CreateAssignment()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssignment(CreateAssignmentViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string filePath = null;

        if (model.File != null && model.File.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);
            filePath = Path.Combine("/uploads", Guid.NewGuid() + Path.GetExtension(model.File.FileName));

            using var stream = new FileStream(_env.WebRootPath + filePath, FileMode.Create);
            await model.File.CopyToAsync(stream);
        }

        var facultyId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var assignment = new Assignment
        {
            Title = model.Title,
            Description = model.Description,
            DueDate = model.DueDate,
            FilePath = filePath,
            CreatedBy = facultyId
        };

        await _assignmentRepo.AddAsync(assignment);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> CreateAssessment()
    { 
        var assignments = await _assignmentRepo.GetAllAsync();
        ViewBag.Assignments = assignments;
        return View(new CreateAssessmentViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssessment(CreateAssessmentViewModel model)
    {
        if (!ModelState.IsValid)
        {ViewBag.Assignments = await _assignmentRepo.GetAllAsync();
            return View(model);
        }
        foreach (var item in model.Criteria)
        {
            var assessment = new Assessment
            {
                AssignmentId = model.AssignmentId,
                Criterion = item.Criterion,
                MaxScore = item.MaxScore
            };

            await _assessmentRepo.AddAsync(assessment);

        }
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Students()
    {
        var students = await _userRepo.GetAllStudentsAsync();
        return View(students);
    }

    public async Task<IActionResult> StudentSubmissions(int id)
    {
        var submissions = await _submissionRepo.GetByStudentIdAsync(id);
        ViewBag.StudentId = id;
        return View(submissions);
    }
}
