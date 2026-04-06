using System.ComponentModel.DataAnnotations;

namespace AcmeGlobalCollege.Models.Entities
{
    public class Assignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(1, 1000)]
        public int MaxScore { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        
        public virtual Course? Course { get; set; }
        public virtual ICollection<AssignmentResult> AssignmentResults { get; set; } = new List<AssignmentResult>();
    }
}