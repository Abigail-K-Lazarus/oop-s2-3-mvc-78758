using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AcmeGlobalCollege.Services.Interfaces;
using AcmeGlobalCollege.Models.ViewModels;

namespace AcmeGlobalCollege.Web.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyController : Controller
    {
        private readonly IFacultyService _facultyService;
        private readonly IGradebookService _gradebookService;
        private readonly IAttendanceService _attendanceService;
        private readonly UserManager<IdentityUser> _userManager;

        public FacultyController(
            IFacultyService facultyService,
            IGradebookService gradebookService,
            IAttendanceService attendanceService,
            UserManager<IdentityUser> userManager)
        {
            _facultyService = facultyService;
            _gradebookService = gradebookService;
            _attendanceService = attendanceService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var faculty = await _facultyService.GetFacultyByUserIdAsync(userId);

            if (faculty == null)
                return RedirectToAction("ContactAdmin", "Home");

            var courses = await _facultyService.GetFacultyCoursesAsync(userId);
            return View(courses);
        }

        public async Task<IActionResult> Gradebook(int courseId)
        {
            var userId = _userManager.GetUserId(User);

            
            var isAuthorized = await _facultyService.IsFacultyAuthorizedForCourseAsync(userId, courseId);
            if (!isAuthorized)
                return RedirectToAction("AccessDenied", "Account");

            var gradebook = await _gradebookService.GetGradebookForCourseAsync(courseId, userId);
            return View(gradebook);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAssignmentResult(int assignmentId, int studentId, int score, string feedback)
        {
            var userId = _userManager.GetUserId(User);

            
            var hasAccess = await _facultyService.IsFacultyAuthorizedForStudentAsync(userId, studentId);
            if (!hasAccess)
                return Json(new { success = false, message = "Unauthorized" });

            await _gradebookService.RecordAssignmentResultAsync(assignmentId, studentId, score, feedback);
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateExamResult(int examId, int studentId, int score)
        {
            var userId = _userManager.GetUserId(User);

            var hasAccess = await _facultyService.IsFacultyAuthorizedForStudentAsync(userId, studentId);
            if (!hasAccess)
                return Json(new { success = false, message = "Unauthorized" });

            await _gradebookService.RecordExamResultAsync(examId, studentId, score);
            return Json(new { success = true });
        }

        public async Task<IActionResult> CourseStudents(int courseId)
        {
            var userId = _userManager.GetUserId(User);

            var isAuthorized = await _facultyService.IsFacultyAuthorizedForCourseAsync(userId, courseId);
            if (!isAuthorized)
                return RedirectToAction("AccessDenied", "Account");

            var students = await _facultyService.GetStudentsByCourseAsync(courseId, userId);
            ViewBag.CourseId = courseId;
            return View(students);
        }

        public async Task<IActionResult> StudentContact(int studentId)
        {
            var userId = _userManager.GetUserId(User);

            var student = await _facultyService.GetStudentWithContactDetailsAsync(studentId, userId);
            if (student == null)
                return RedirectToAction("AccessDenied", "Account");

            return View(student);
        }

        public async Task<IActionResult> CourseAttendance(int courseId)
        {
            var userId = _userManager.GetUserId(User);

            var isAuthorized = await _facultyService.IsFacultyAuthorizedForCourseAsync(userId, courseId);
            if (!isAuthorized)
                return RedirectToAction("AccessDenied", "Account");

            var students = await _facultyService.GetStudentsByCourseAsync(courseId, userId);
            ViewBag.CourseId = courseId;
            return View(students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordAttendance(int courseEnrolmentId, int weekNumber, bool isPresent, string? notes)
        {
            var userId = _userManager.GetUserId(User);


            await _attendanceService.RecordAttendanceAsync(courseEnrolmentId, weekNumber, DateTime.Today, isPresent, notes);
            return Json(new { success = true });
        }
    }
}