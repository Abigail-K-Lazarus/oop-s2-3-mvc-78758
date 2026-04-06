using Microsoft.EntityFrameworkCore;
using AcmeGlobalCollege.Data;
using AcmeGlobalCollege.Models.Entities;
using AcmeGlobalCollege.Services.Interfaces;
using System.Linq;

namespace AcmeGlobalCollege.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<IEnumerable<Branch>> GetAllBranchesAsync()
        {
            return await _context.Branches.ToListAsync();
        }

        public async Task<Branch?> GetBranchByIdAsync(int id)
        {
            return await _context.Branches.FindAsync(id);
        }

        public async Task<Branch> CreateBranchAsync(Branch branch)
        {
            await _context.Branches.AddAsync(branch);
            await _context.SaveChangesAsync();
            return branch;
        }

        public async Task UpdateBranchAsync(Branch branch)
        {
            _context.Branches.Update(branch);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBranchAsync(int id)
        {
            var branch = await GetBranchByIdAsync(id);
            if (branch != null)
            {
                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();
            }
        }

        
        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.Branch)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Branch)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await GetCourseByIdAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }

        
        public async Task AssignFacultyToCourseAsync(int courseId, int facultyProfileId)
        {
            var exists = await _context.CourseFaculties
                .AnyAsync(cf => cf.CourseId == courseId && cf.FacultyProfileId == facultyProfileId);

            if (!exists)
            {
                await _context.CourseFaculties.AddAsync(new CourseFaculty
                {
                    CourseId = courseId,
                    FacultyProfileId = facultyProfileId,
                    AssignedDate = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFacultyFromCourseAsync(int courseId, int facultyProfileId)
        {
            var assignment = await _context.CourseFaculties
                .FirstOrDefaultAsync(cf => cf.CourseId == courseId && cf.FacultyProfileId == facultyProfileId);

            if (assignment != null)
            {
                _context.CourseFaculties.Remove(assignment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<FacultyProfile>> GetAvailableFacultyAsync()
        {
            return await _context.FacultyProfiles
                .Include(f => f.IdentityUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseFaculty>> GetCourseFacultyAssignmentsAsync(int courseId)
        {
            return await _context.CourseFaculties
                .Include(cf => cf.FacultyProfile)
                    .ThenInclude(f => f.IdentityUser)
                .Where(cf => cf.CourseId == courseId)
                .ToListAsync();
        }

        
        public async Task EnrolStudentInCourseAsync(int studentId, int courseId)
        {
            var exists = await _context.CourseEnrolments
                .AnyAsync(ce => ce.StudentProfileId == studentId && ce.CourseId == courseId);

            if (!exists)
            {
                await _context.CourseEnrolments.AddAsync(new CourseEnrolment
                {
                    StudentProfileId = studentId,
                    CourseId = courseId,
                    EnrolDate = DateTime.UtcNow,
                    Status = "Enrolled"
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task WithdrawStudentFromCourseAsync(int studentId, int courseId)
        {
            var enrolment = await _context.CourseEnrolments
                .FirstOrDefaultAsync(ce => ce.StudentProfileId == studentId && ce.CourseId == courseId);

            if (enrolment != null)
            {
                enrolment.Status = "Dropped";
                _context.CourseEnrolments.Update(enrolment);
                await _context.SaveChangesAsync();
            }
        }

        
        public async Task ReleaseExamResultsAsync(int examId, bool release)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam != null)
            {
                exam.ResultsReleased = release;
                _context.Exams.Update(exam);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> AreExamResultsReleasedAsync(int examId)
        {
            var exam = await _context.Exams.FindAsync(examId);
            return exam?.ResultsReleased ?? false;
        }
    }
}
