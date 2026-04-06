using System.ComponentModel.DataAnnotations;

namespace AcmeGlobalCollege.Models.Entities
{
    public class AttendanceRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseEnrolmentId { get; set; }

        [Required]
        public int WeekNumber { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public bool IsPresent { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual CourseEnrolment? CourseEnrolment { get; set; }
    }
}