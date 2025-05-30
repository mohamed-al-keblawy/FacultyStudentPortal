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
    private readonly IWebHostEnvironment _env;

    public FacultyController(IAssignmentRepository assignmentRepo, IWebHostEnvironment env)
    {
        _assignmentRepo = assignmentRepo;
        _env = env;
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
}
