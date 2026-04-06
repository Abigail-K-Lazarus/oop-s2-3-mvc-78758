using System.ComponentModel.DataAnnotations;

namespace AcmeGlobalCollege.Models.Entities
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
        public virtual ICollection<StudentProfile> Students { get; set; } = new List<StudentProfile>();
        public virtual ICollection<FacultyProfile> Faculties { get; set; } = new List<FacultyProfile>();
    }
}