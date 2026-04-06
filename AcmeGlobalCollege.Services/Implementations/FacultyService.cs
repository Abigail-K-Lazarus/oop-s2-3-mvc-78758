using Microsoft.EntityFrameworkCore;
using AcmeGlobalCollege.Data;
using AcmeGlobalCollege.Models.Entities;
using AcmeGlobalCollege.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace AcmeGlobalCollege.Services.Implementations
{
    public class FacultyService : IFacultyService
    {
        private readonly ApplicationDbContext _context;

        public FacultyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FacultyProfile?> GetFacultyByUserIdAsync(string userId)
        {
            return await _context.FacultyProfiles
                .Include(f => f.Branch)
                .Include(f => f.IdentityUser)
                .Include(f => f.CourseFaculties)
                    .ThenInclude(cf => cf.Course)
                .FirstOrDefaultAsync(f => f.IdentityUserId == userId);
        }

        public async Task<IEnumerable<Course>> GetFacultyCoursesAsync(string userId)
        {
            var faculty = await GetFacultyByUserIdAsync(userId);
            if (faculty == null) return new List<Course>();

            return await _context.CourseFaculties
                .Include(cf => cf.Course)
                .ThenInclude(c => c.Branch)
                .Where(cf => cf.FacultyProfileId == faculty.Id)
                .Select(cf => cf.Course)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentProfile>> GetStudentsByCourseAsync(int courseId, string facultyUserId)
        {
            
            var isAuthorized = await IsFacultyAuthorizedForCourseAsync(facultyUserId, courseId);
            if (!isAuthorized) return new List<StudentProfile>();

            return await _context.CourseEnrolments
                .Include(ce => ce.StudentProfile)
                    .ThenInclude(s => s.Branch)
                .Include(ce => ce.StudentProfile)
                    .ThenInclude(s => s.IdentityUser)
                .Where(ce => ce.CourseId == courseId && ce.Status == "Enrolled")
                .Select(ce => ce.StudentProfile)
                .ToListAsync();
        }

        public async Task<StudentProfile?> GetStudentWithContactDetailsAsync(int studentId, string facultyUserId)
        {
            var student = await _context.StudentProfiles
                .Include(s => s.IdentityUser)
                .Include(s => s.Branch)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null) return null;

            
            var hasAccess = await IsFacultyAuthorizedForStudentAsync(facultyUserId, studentId);
            return hasAccess ? student : null;
        }

        public async Task<bool> IsFacultyAuthorizedForStudentAsync(string facultyUserId, int studentId)
        {
            var faculty = await GetFacultyByUserIdAsync(facultyUserId);
            if (faculty == null) return false;

            
            var facultyCourseIds = await _context.CourseFaculties
                .Where(cf => cf.FacultyProfileId == faculty.Id)
                .Select(cf => cf.CourseId)
                .ToListAsync();

            
            return await _context.CourseEnrolments
                .AnyAsync(ce => ce.StudentProfileId == studentId &&
                               facultyCourseIds.Contains(ce.CourseId));
        }

        public async Task<bool> IsFacultyAuthorizedForCourseAsync(string facultyUserId, int courseId)
        {
            var faculty = await GetFacultyByUserIdAsync(facultyUserId);
            if (faculty == null) return false;

            return await _context.CourseFaculties
                .AnyAsync(cf => cf.FacultyProfileId == faculty.Id && cf.CourseId == courseId);
        }
    }
}
