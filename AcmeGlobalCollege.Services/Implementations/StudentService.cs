using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AcmeGlobalCollege.Data;
using AcmeGlobalCollege.Models.Entities;
using AcmeGlobalCollege.Models.ViewModels;
using AcmeGlobalCollege.Services.Interfaces;

namespace AcmeGlobalCollege.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StudentProfile?> GetStudentByIdAsync(int id)
        {
            return await _context.StudentProfiles
                .Include(s => s.Branch)
                .Include(s => s.IdentityUser)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<StudentProfile?> GetStudentByUserIdAsync(string userId)
        {
            return await _context.StudentProfiles
                .Include(s => s.Branch)
                .Include(s => s.IdentityUser)
                .FirstOrDefaultAsync(s => s.IdentityUserId == userId);
        }

        public async Task<IEnumerable<StudentProfile>> GetAllStudentsAsync()
        {
            return await _context.StudentProfiles
                .Include(s => s.Branch)
                .Include(s => s.IdentityUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentProfile>> GetStudentsByBranchAsync(int branchId)
        {
            return await _context.StudentProfiles
                .Include(s => s.Branch)
                .Where(s => s.BranchId == branchId)
                .ToListAsync();
        }

        public async Task<StudentProfile> CreateStudentAsync(StudentProfile student)
        {
            if (string.IsNullOrEmpty(student.StudentNumber))
            {
                student.StudentNumber = GenerateStudentNumber();
            }

            student.EnrolmentDate = DateTime.UtcNow;
            student.Status = "Active";

            await _context.StudentProfiles.AddAsync(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task UpdateStudentAsync(StudentProfile student)
        {
            _context.StudentProfiles.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await GetStudentByIdAsync(id);
            if (student != null)
            {
                _context.StudentProfiles.Remove(student);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CourseEnrolment>> GetStudentEnrolmentsAsync(int studentId)
        {
            return await _context.CourseEnrolments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Branch)
                .Where(e => e.StudentProfileId == studentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<AttendanceRecord>> GetStudentAttendanceAsync(int studentId)
        {
            return await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(ce => ce.Course)
                .Where(a => a.CourseEnrolment.StudentProfileId == studentId)
                .ToListAsync();
        }

        public async Task<StudentDashboardViewModel> GetStudentDashboardAsync(string userId)
        {
            var student = await GetStudentByUserIdAsync(userId);
            if (student == null)
                return new StudentDashboardViewModel();

            var enrolments = await GetStudentEnrolmentsAsync(student.Id);
            var attendance = await GetStudentAttendanceAsync(student.Id);

            
            var attendanceList = attendance.ToList();  
            var totalAttendance = attendanceList.Count;  
            var presentAttendance = attendanceList.Count(a => a.IsPresent); 

            var overallAttendance = totalAttendance > 0 ? (decimal)presentAttendance / totalAttendance * 100 : 0;

            
            var assignmentResults = await _context.AssignmentResults
                .Include(ar => ar.Assignment)
                .Where(ar => ar.StudentProfileId == student.Id)
                .ToListAsync();

            
            var examResults = await _context.ExamResults
                .Include(er => er.Exam)
                .Where(er => er.StudentProfileId == student.Id && er.Exam.ResultsReleased == true)
                .ToListAsync();

            
            var courseProgress = new Dictionary<int, decimal>(); 
            foreach (var enrolment in enrolments)
            {
                var grade = await GetStudentOverallGradeAsync(student.Id, enrolment.CourseId);
                courseProgress[enrolment.CourseId] = grade; 
            }

            return new StudentDashboardViewModel
            {
                Student = student,
                CurrentEnrolments = enrolments.ToList(),
                RecentAttendance = attendanceList.OrderByDescending(a => a.Date).Take(10).ToList(),
                AssignmentResults = assignmentResults,
                ReleasedExamResults = examResults,
                CourseProgress = courseProgress,
                OverallAttendancePercentage = overallAttendance
            };
        }

        public async Task<decimal> GetAttendancePercentageAsync(int studentId, int courseId)
        {
            var attendance = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                .Where(a => a.CourseEnrolment.StudentProfileId == studentId &&
                           a.CourseEnrolment.CourseId == courseId)
                .ToListAsync();

            if (!attendance.Any()) return 0;

            var present = attendance.Count(a => a.IsPresent);
            return (decimal)present / attendance.Count * 100;
        }

        private async Task<decimal> GetStudentOverallGradeAsync(int studentId, int courseId)
        {
            var assignments = await _context.Assignments
                .Where(a => a.CourseId == courseId)
                .ToListAsync();

            var exams = await _context.Exams
                .Where(e => e.CourseId == courseId)
                .ToListAsync();

            decimal totalWeightedScore = 0;
            decimal totalMaxScore = 0;

            foreach (var assignment in assignments)
            {
                var result = await _context.AssignmentResults
                    .FirstOrDefaultAsync(ar => ar.AssignmentId == assignment.Id && ar.StudentProfileId == studentId);

                if (result != null)
                {
                    totalWeightedScore += (decimal)result.Score / assignment.MaxScore * 100;
                    totalMaxScore += 100;
                }
            }

            foreach (var exam in exams)
            {
                var result = await _context.ExamResults
                    .FirstOrDefaultAsync(er => er.ExamId == exam.Id && er.StudentProfileId == studentId);

                if (result != null)
                {
                    totalWeightedScore += (decimal)result.Score / exam.MaxScore * 100;
                    totalMaxScore += 100;
                }
            }

            return totalMaxScore > 0 ? totalWeightedScore / totalMaxScore * 100 : 0;
        }

        private string GenerateStudentNumber()
        {
            var year = DateTime.Now.Year;
            var count = _context.StudentProfiles.Count() + 1;
            return $"STU-{year}-{count:D4}";
        }

        public Task<string?> GetStudentDashboardAsync(string? userId, object attendance)
        {
            throw new NotImplementedException();
        }
    }
}