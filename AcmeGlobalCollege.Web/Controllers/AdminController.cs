using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcmeGlobalCollege.Data;
using AcmeGlobalCollege.Models.Entities;
using AcmeGlobalCollege.Models.ViewModels;
using AcmeGlobalCollege.Services.Interfaces;


namespace AcmeGlobalCollege.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IStudentService _studentService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(
            IAdminService adminService,
            IStudentService studentService,
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _adminService = adminService;
            _studentService = studentService;
            _context = context;
            _userManager = userManager;
        }

        // ==================== CREATE FACULTY ====================
        [HttpGet]
        public async Task<IActionResult> CreateFaculty()
        {
            ViewBag.Branches = await _adminService.GetAllBranchesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFaculty(CreateFacultyViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create Identity user
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Faculty");

                   
                    var facultyProfile = new FacultyProfile
                    {
                        IdentityUserId = user.Id,
                        FacultyNumber = GenerateFacultyNumber(),
                        Name = model.Name,
                        Email = model.Email,
                        Phone = model.Phone,
                        BranchId = model.BranchId,
                        Department = model.Department,
                        Specialization = model.Specialization,
                        HireDate = DateTime.UtcNow
                    };

                    _context.FacultyProfiles.Add(facultyProfile);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Faculties));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Branches = await _adminService.GetAllBranchesAsync();
            return View(model);
        }

        // ==================== FACULTIES LIST ====================
        public async Task<IActionResult> Faculties()
        {
            var faculties = await _context.FacultyProfiles
                .Include(f => f.IdentityUser)
                .Include(f => f.Branch)
                .ToListAsync();
            return View(faculties);
        }

        // ==================== HELPER METHODS ====================
        private string GenerateFacultyNumber()
        {
            var year = DateTime.Now.Year;
            var random = new Random().Next(1000, 9999);
            return $"FAC-{year}-{random}";
        }
    }
}