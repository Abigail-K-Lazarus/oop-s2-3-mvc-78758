using AcmeGlobalCollege.Models.Entities;

namespace AcmeGlobalCollege.Models.ViewModels
{
    public class GradebookViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public List<StudentGradeViewModel> Students { get; set; } = new();
    }

    public class StudentGradeViewModel
    {
        public int StudentProfileId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<AssignmentGradeViewModel> AssignmentGrades { get; set; } = new();
        public List<ExamGradeViewModel> ExamGrades { get; set; } = new();
        public decimal? OverallGrade { get; set; }
    }

    public class AssignmentGradeViewModel
    {
        public int AssignmentId { get; set; }
        public string AssignmentTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }

    public class ExamGradeViewModel
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public string Grade { get; set; } = string.Empty;
        public bool ResultsReleased { get; set; }
    }
}