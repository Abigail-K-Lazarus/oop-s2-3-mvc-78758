using AcmeGlobalCollege.Models.Entities;
using AcmeGlobalCollege.Models.ViewModels;

namespace AcmeGlobalCollege.Services.Interfaces
{
    public interface IGradebookService
    {
        Task<GradebookViewModel> GetGradebookForCourseAsync(int courseId, string facultyUserId);
        Task<AssignmentResult?> RecordAssignmentResultAsync(int assignmentId, int studentId, int score, string feedback);
        Task<ExamResult?> RecordExamResultAsync(int examId, int studentId, int score);
        Task<IEnumerable<Assignment>> GetCourseAssignmentsAsync(int courseId);
        Task<IEnumerable<Exam>> GetCourseExamsAsync(int courseId);
        Task UpdateAssignmentResultAsync(int resultId, int score, string feedback);
        Task<decimal> CalculateStudentOverallGradeAsync(int studentId, int courseId);
    }
}