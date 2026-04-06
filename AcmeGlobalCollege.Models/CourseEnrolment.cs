using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcmeGlobalCollege.Models.Entities
{
    public class CourseEnrolment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentProfileId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DateTime EnrolDate { get; set; } = DateTime.UtcNow;

        [StringLength(20)]
        public string Status { get; set; } = "Enrolled"; 

        [Column(TypeName = "decimal(5,2)")]
        public decimal? FinalGrade { get; set; }

        
        [ForeignKey("StudentProfileId")]
        public virtual StudentProfile? StudentProfile { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}