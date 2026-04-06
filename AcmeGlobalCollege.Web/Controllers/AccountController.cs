using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AcmeGlobalCollege.Models.Entities;
using AcmeGlobalCollege.Models.ViewModels;
using AcmeGlobalCollege.Services.Interfaces;

namespace AcmeGlobalCollege.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStudentService _studentService;
        private readonly IFacultyService _facultyService;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IStudentService studentService,
            IFacultyService facultyService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _studentService = studentService;
            _facultyService = facultyService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);

                    // Redirect based on role
                    if (roles.Contains("Admin"))
                        return RedirectToAction("Index", "Admin");
                    else if (roles.Contains("Faculty"))
                        return RedirectToAction("Dashboard", "Faculty");
                    else if (roles.Contains("Student"))
                        return RedirectToAction("Dashboard", "Student");
                    else
                        return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string role = "Student")
        {
            ViewBag.Role = role;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string role = "Student")
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true  // Auto-confirm for demo
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }

                    await _userManager.AddToRoleAsync(user, role);

                    
                    if (role == "Student")
                    {
                        var student = new StudentProfile
                        {
                            IdentityUserId = user.Id,
                            StudentNumber = GenerateStudentNumber(),
                            Name = model.FullName,
                            Email = model.Email,
                            Phone = model.Phone,
                            Address = model.Address,
                            DateOfBirth = model.DateOfBirth ?? DateTime.Now.AddYears(-18),
                            BranchId = model.BranchId,
                            Status = "Active"
                        };
                        await _studentService.CreateStudentAsync(student);
                    }
                    else if (role == "Faculty")
                    {
                        
                        
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (role == "Student")
                        return RedirectToAction("Dashboard", "Student");
                    else
                        return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Role = role;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private string GenerateStudentNumber()
        {
            var year = DateTime.Now.Year;
            var random = new Random().Next(1000, 9999);
            return $"STU-{year}-{random}";
        }
    }
}