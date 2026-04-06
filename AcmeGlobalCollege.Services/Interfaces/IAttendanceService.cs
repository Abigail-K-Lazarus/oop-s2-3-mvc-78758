using AcmeGlobalCollege.Models.Entities;

namespace AcmeGlobalCollege.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<AttendanceRecord?> RecordAttendanceAsync(int courseEnrolmentId, int weekNumber, DateTime date, bool isPresent, string? notes);
        Task<IEnumerable<AttendanceRecord>> GetAttendanceByCourseAsync(int courseId, int weekNumber);
        Task<IEnumerable<AttendanceRecord>> GetAttendanceByStudentAsync(int studentId, int courseId);
        Task<decimal> GetCourseAttendancePercentageAsync(int courseId);
        Task<Dictionary<int, decimal>> GetStudentAttendanceSummaryAsync(int studentId);
    }
}