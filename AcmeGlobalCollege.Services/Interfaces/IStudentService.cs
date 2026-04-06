using AcmeGlobalCollege.Models.Entities;
using AcmeGlobalCollege.Models.ViewModels;

namespace AcmeGlobalCollege.Services.Interfaces
{
    public interface IStudentService
    {
        Task<StudentProfile?> GetStudentByIdAsync(int id);
        Task<StudentProfile?> GetStudentByUserIdAsync(string userId);
        Task<IEnumerable<StudentProfile>> GetAllStudentsAsync();
        Task<IEnumerable<StudentProfile>> GetStudentsByBranchAsync(int branchId);
        Task<StudentProfile> CreateStudentAsync(StudentProfile student);
        Task UpdateStudentAsync(StudentProfile student);
        Task DeleteStudentAsync(int id);
        Task<IEnumerable<CourseEnrolment>> GetStudentEnrolmentsAsync(int studentId);
        Task<IEnumerable<AttendanceRecord>> GetStudentAttendanceAsync(int studentId);
        Task<StudentDashboardViewModel> GetStudentDashboardAsync(string userId);
        Task<decimal> GetAttendancePercentageAsync(int studentId, int courseId);
        Task<string?> GetStudentDashboardAsync(string? userId, object attendance);
    }
}
