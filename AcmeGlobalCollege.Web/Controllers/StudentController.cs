using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AcmeGlobalCollege.Models.Entities;
using AcmeGlobalCollege.Models.ViewModels;
using AcmeGlobalCollege.Services.Interfaces;

namespace AcmeGlobalCollege.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentController(IStudentService studentService, UserManager<IdentityUser> userManager)
        {
            _studentService = studentService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var dashboard = await _studentService.GetStudentDashboardAsync(userId);

            
            if (dashboard.Student == null || dashboard.Student.Id == 0)
            {
                return RedirectToAction("CompleteProfile", "Student");
            }

            return View(dashboard);
        }

       
    }
}