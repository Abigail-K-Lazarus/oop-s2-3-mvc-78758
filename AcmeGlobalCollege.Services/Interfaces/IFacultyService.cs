using AcmeGlobalCollege.Models.Entities;

namespace AcmeGlobalCollege.Services.Interfaces
{
    public interface IFacultyService
    {
        Task<FacultyProfile?> GetFacultyByUserIdAsync(string userId);
        Task<IEnumerable<Course>> GetFacultyCoursesAsync(string userId);
        Task<IEnumerable<StudentProfile>> GetStudentsByCourseAsync(int courseId, string facultyUserId);
        Task<StudentProfile?> GetStudentWithContactDetailsAsync(int studentId, string facultyUserId);
        Task<bool> IsFacultyAuthorizedForStudentAsync(string facultyUserId, int studentId);
        Task<bool> IsFacultyAuthorizedForCourseAsync(string facultyUserId, int courseId);
    }
}