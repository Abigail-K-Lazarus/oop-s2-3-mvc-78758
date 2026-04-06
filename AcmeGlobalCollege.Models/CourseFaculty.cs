using System.ComponentModel.DataAnnotations;

namespace AcmeGlobalCollege.Models.Entities
{
    public class CourseFaculty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int FacultyProfileId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        public bool IsPrimary { get; set; } = false;

        
        public virtual Course? Course { get; set; }
        public virtual FacultyProfile? FacultyProfile { get; set; }
    }
}