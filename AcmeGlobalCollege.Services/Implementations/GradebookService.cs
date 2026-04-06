using Microsoft.EntityFrameworkCore;
using AcmeGlobalCollege.Data;
using AcmeGlobalCollege.Models.Entities;
using AcmeGlobalCollege.Models.ViewModels;
using AcmeGlobalCollege.Services.Interfaces;
using System.Linq;

namespace AcmeGlobalCollege.Services.Implementations
{
    public class GradebookService : IGradebookService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFacultyService _facultyService;

        public GradebookService(ApplicationDbContext context, IFacultyService facultyService)
        {
            _context = context;
            _facultyService = facultyService;
        }

        public async Task<GradebookViewModel> GetGradebookForCourseAsync(int courseId, string facultyUserId)
        {
            
            var isAuthorized = await _facultyService.IsFacultyAuthorizedForCourseAsync(facultyUserId, courseId);
            if (!isAuthorized) throw new UnauthorizedAccessException("Not authorized for this course");

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) throw new Exception("Course not found");

            var students = await _facultyService.GetStudentsByCourseAsync(courseId, facultyUserId);
            var assignments = await GetCourseAssignmentsAsync(courseId);
            var exams = await GetCourseExamsAsync(courseId);

            var studentGrades = new List<StudentGradeViewModel>();

            foreach (var student in students)
            {
                var studentGrade = new StudentGradeViewModel
                {
                    StudentProfileId = student.Id,
                    StudentName = student.Name,
                    StudentNumber = student.StudentNumber,
                    Email = student.Email,
                    Phone = student.Phone ?? "N/A",
                    AssignmentGrades = new List<AssignmentGradeViewModel>(),
                    ExamGrades = new List<ExamGradeViewModel>()
                };

                
                foreach (var assignment in assignments)
                {
                    var result = await _context.AssignmentResults
                        .FirstOrDefaultAsync(ar => ar.AssignmentId == assignment.Id &&
                                                   ar.StudentProfileId == student.Id);

                    studentGrade.AssignmentGrades.Add(new AssignmentGradeViewModel
                    {
                        AssignmentId = assignment.Id,
                        AssignmentTitle = assignment.Title,
                        Score = result?.Score ?? 0,
                        MaxScore = assignment.MaxScore,
                        Feedback = result?.Feedback ?? "Not submitted"
                    });
                }

                
                foreach (var exam in exams)
                {
                    var result = await _context.ExamResults
                        .FirstOrDefaultAsync(er => er.ExamId == exam.Id &&
                                                   er.StudentProfileId == student.Id);

                    studentGrade.ExamGrades.Add(new ExamGradeViewModel
                    {
                        ExamId = exam.Id,
                        ExamTitle = exam.Title,
                        Score = result?.Score ?? 0,
                        MaxScore = exam.MaxScore,
                        Grade = result?.Grade ?? "N/A",
                        ResultsReleased = exam.ResultsReleased
                    });
                }

                studentGrade.OverallGrade = await CalculateStudentOverallGradeAsync(student.Id, courseId);
                studentGrades.Add(studentGrade);
            }

            return new GradebookViewModel
            {
                CourseId = courseId,
                CourseName = course.Name,
                Students = studentGrades
            };
        }

        public async Task<AssignmentResult?> RecordAssignmentResultAsync(int assignmentId, int studentId, int score, string feedback)
        {
            var assignment = await _context.Assignments.FindAsync(assignmentId);
            if (assignment == null) return null;

            if (score > assignment.MaxScore) score = assignment.MaxScore;
            if (score < 0) score = 0;

            var existing = await _context.AssignmentResults
                .FirstOrDefaultAsync(ar => ar.AssignmentId == assignmentId &&
                                          ar.StudentProfileId == studentId);

            if (existing != null)
            {
                existing.Score = score;
                existing.Feedback = feedback;
                existing.SubmittedAt = DateTime.UtcNow;
                _context.AssignmentResults.Update(existing);
            }
            else
            {
                existing = new AssignmentResult
                {
                    AssignmentId = assignmentId,
                    StudentProfileId = studentId,
                    Score = score,
                    Feedback = feedback,
                    SubmittedAt = DateTime.UtcNow
                };
                await _context.AssignmentResults.AddAsync(existing);
            }

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<ExamResult?> RecordExamResultAsync(int examId, int studentId, int score)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam == null) return null;

            if (score > exam.MaxScore) score = exam.MaxScore;
            if (score < 0) score = 0;

            var grade = CalculateGrade((decimal)score / exam.MaxScore * 100);

            var existing = await _context.ExamResults
                .FirstOrDefaultAsync(er => er.ExamId == examId &&
                                          er.StudentProfileId == studentId);

            if (existing != null)
            {
                existing.Score = score;
                existing.Grade = grade;
                existing.SubmittedAt = DateTime.UtcNow;
                _context.ExamResults.Update(existing);
            }
            else
            {
                existing = new ExamResult
                {
                    ExamId = examId,
                    StudentProfileId = studentId,
                    Score = score,
                    Grade = grade,
                    SubmittedAt = DateTime.UtcNow
                };
                await _context.ExamResults.AddAsync(existing);
            }

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<IEnumerable<Assignment>> GetCourseAssignmentsAsync(int courseId)
        {
            return await _context.Assignments
                .Where(a => a.CourseId == courseId)
                .OrderBy(a => a.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetCourseExamsAsync(int courseId)
        {
            return await _context.Exams
                .Where(e => e.CourseId == courseId)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task UpdateAssignmentResultAsync(int resultId, int score, string feedback)
        {
            var result = await _context.AssignmentResults.FindAsync(resultId);
            if (result != null)
            {
                result.Score = score;
                result.Feedback = feedback;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> CalculateStudentOverallGradeAsync(int studentId, int courseId)
        {
            var assignments = await GetCourseAssignmentsAsync(courseId);
            var exams = await GetCourseExamsAsync(courseId);

            decimal totalPoints = 0;
            decimal earnedPoints = 0;

            foreach (var assignment in assignments)
            {
                var result = await _context.AssignmentResults
                    .FirstOrDefaultAsync(ar => ar.AssignmentId == assignment.Id &&
                                              ar.StudentProfileId == studentId);

                totalPoints += assignment.MaxScore;
                earnedPoints += result?.Score ?? 0;
            }

            foreach (var exam in exams)
            {
                var result = await _context.ExamResults
                    .FirstOrDefaultAsync(er => er.ExamId == exam.Id &&
                                              er.StudentProfileId == studentId);

                totalPoints += exam.MaxScore;
                earnedPoints += result?.Score ?? 0;
            }

            return totalPoints > 0 ? earnedPoints / totalPoints * 100 : 0;
        }

        private string CalculateGrade(decimal percentage)
        {
            if (percentage >= 90) return "A";
            if (percentage >= 80) return "B";
            if (percentage >= 70) return "C";
            if (percentage >= 60) return "D";
            return "F";
        }
    }
}