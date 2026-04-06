using Microsoft.EntityFrameworkCore;
using AcmeGlobalCollege.Data;
using AcmeGlobalCollege.Models.Entities;
using AcmeGlobalCollege.Services.Interfaces;
using System.Linq;

namespace AcmeGlobalCollege.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;

        public AttendanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AttendanceRecord?> RecordAttendanceAsync(int courseEnrolmentId, int weekNumber, DateTime date, bool isPresent, string? notes)
        {
            var existing = await _context.AttendanceRecords
                .FirstOrDefaultAsync(a => a.CourseEnrolmentId == courseEnrolmentId &&
                                         a.WeekNumber == weekNumber);

            if (existing != null)
            {
                existing.IsPresent = isPresent;
                existing.Notes = notes;
                existing.Date = date;
                _context.AttendanceRecords.Update(existing);
            }
            else
            {
                existing = new AttendanceRecord
                {
                    CourseEnrolmentId = courseEnrolmentId,
                    WeekNumber = weekNumber,
                    Date = date,
                    IsPresent = isPresent,
                    Notes = notes
                };
                await _context.AttendanceRecords.AddAsync(existing);
            }

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<AttendanceRecord>> GetAttendanceByCourseAsync(int courseId, int weekNumber)
        {
            return await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(ce => ce.StudentProfile)
                .Where(a => a.CourseEnrolment.CourseId == courseId && a.WeekNumber == weekNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<AttendanceRecord>> GetAttendanceByStudentAsync(int studentId, int courseId)
        {
            return await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                .Where(a => a.CourseEnrolment.StudentProfileId == studentId &&
                           a.CourseEnrolment.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<decimal> GetCourseAttendancePercentageAsync(int courseId)
        {
            var allAttendance = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                .Where(a => a.CourseEnrolment.CourseId == courseId)
                .ToListAsync();

            if (!allAttendance.Any()) return 0;

            var present = allAttendance.Count(a => a.IsPresent);
            return (decimal)present / allAttendance.Count * 100;
        }

        public async Task<Dictionary<int, decimal>> GetStudentAttendanceSummaryAsync(int studentId)
        {
            var attendance = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(ce => ce.Course)
                .Where(a => a.CourseEnrolment.StudentProfileId == studentId)
                .ToListAsync();

            var summary = new Dictionary<int, decimal>();
            var courseGroups = attendance.GroupBy(a => a.CourseEnrolment.CourseId);

            foreach (var group in courseGroups)
            {
                var present = group.Count(a => a.IsPresent);
                var total = group.Count();
                var percentage = total > 0 ? (decimal)present / total * 100 : 0;
                summary[group.Key] = percentage;
            }

            return summary;
        }
    }
}