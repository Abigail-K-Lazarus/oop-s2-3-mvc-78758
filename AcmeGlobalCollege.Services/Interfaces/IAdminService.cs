using AcmeGlobalCollege.Models.Entities;

namespace AcmeGlobalCollege.Services.Interfaces
{
    public interface IAdminService
    {
        
        Task<IEnumerable<Branch>> GetAllBranchesAsync();
        Task<Branch?> GetBranchByIdAsync(int id);
        Task<Branch> CreateBranchAsync(Branch branch);
        Task UpdateBranchAsync(Branch branch);
        Task DeleteBranchAsync(int id);

       
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<Course> CreateCourseAsync(Course course);
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int id);

        
        Task AssignFacultyToCourseAsync(int courseId, int facultyProfileId);
        Task RemoveFacultyFromCourseAsync(int courseId, int facultyProfileId);
        Task<IEnumerable<FacultyProfile>> GetAvailableFacultyAsync();
        Task<IEnumerable<CourseFaculty>> GetCourseFacultyAssignmentsAsync(int courseId);

        
        Task EnrolStudentInCourseAsync(int studentId, int courseId);
        Task WithdrawStudentFromCourseAsync(int studentId, int courseId);

        
        Task ReleaseExamResultsAsync(int examId, bool release);
        Task<bool> AreExamResultsReleasedAsync(int examId);
    }
}