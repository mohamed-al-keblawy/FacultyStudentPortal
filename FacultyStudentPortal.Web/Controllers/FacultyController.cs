using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacultyStudentPortal.Web.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
