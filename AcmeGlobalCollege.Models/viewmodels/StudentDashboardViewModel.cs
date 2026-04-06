using AcmeGlobalCollege.Models.Entities;
using System.Collections.Generic;

namespace AcmeGlobalCollege.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public StudentProfile Student { get; set; } = new StudentProfile();
        public List<CourseEnrolment> CurrentEnrolments { get; set; } = new List<CourseEnrolment>();
        public List<AttendanceRecord> RecentAttendance { get; set; } = new List<AttendanceRecord>();
        public List<AssignmentResult> AssignmentResults { get; set; } = new List<AssignmentResult>();
        public List<ExamResult> ReleasedExamResults { get; set; } = new List<ExamResult>();

        
        public Dictionary<int, decimal> CourseProgress { get; set; } = new Dictionary<int, decimal>();

        public decimal OverallAttendancePercentage { get; set; }
    }
}