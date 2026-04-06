using System.ComponentModel.DataAnnotations;

namespace AcmeGlobalCollege.Models.Entities
{
    public class Exam
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, 1000)]
        public int MaxScore { get; set; }

        public bool ResultsReleased { get; set; } = false;

        
        public virtual Course? Course { get; set; }
        public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
    }
}