using AcmeGlobalCollege.Models.Entities;

namespace AcmeGlobalCollege.Models.ViewModels
{
    public class EnrolmentViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public List<Course> AvailableCourses { get; set; } = new();
        public List<CourseEnrolment> CurrentEnrolments { get; set; } = new();
    }

    public class EnrolCourseViewModel
    {
        public int StudentProfileId { get; set; }
        public int CourseId { get; set; }
    }
}
