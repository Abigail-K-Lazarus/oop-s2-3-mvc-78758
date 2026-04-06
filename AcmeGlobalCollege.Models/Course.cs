using System.ComponentModel.DataAnnotations;

namespace AcmeGlobalCollege.Models.Entities
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        public int Credits { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Range(1, 100)]
        public int MaxCapacity { get; set; } = 30;

        // Navigation properties
        public virtual Branch? Branch { get; set; }
        public virtual ICollection<CourseEnrolment> Enrolments { get; set; } = new List<CourseEnrolment>();
        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
        public virtual ICollection<CourseFaculty> CourseFaculties { get; set; } = new List<CourseFaculty>();
    }
}