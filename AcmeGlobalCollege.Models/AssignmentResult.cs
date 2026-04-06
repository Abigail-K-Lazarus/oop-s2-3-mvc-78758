using System.ComponentModel.DataAnnotations;

namespace AcmeGlobalCollege.Models.Entities
{
    public class AssignmentResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        [Required]
        public int StudentProfileId { get; set; }

        [Required]
        [Range(0, 1000)]
        public int Score { get; set; }

        [StringLength(500)]
        public string? Feedback { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

       
        public virtual Assignment? Assignment { get; set; }
        public virtual StudentProfile? StudentProfile { get; set; }
    }
}