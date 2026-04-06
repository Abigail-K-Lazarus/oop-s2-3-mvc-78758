using System.ComponentModel.DataAnnotations;

namespace AcmeGlobalCollege.Models.Entities
{
    public class ExamResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public int StudentProfileId { get; set; }

        [Required]
        [Range(0, 1000)]
        public int Score { get; set; }

        [StringLength(2)]
        public string? Grade { get; set; } // A, B, C, D, F

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        
        public virtual Exam? Exam { get; set; }
        public virtual StudentProfile? StudentProfile { get; set; }
    }
}