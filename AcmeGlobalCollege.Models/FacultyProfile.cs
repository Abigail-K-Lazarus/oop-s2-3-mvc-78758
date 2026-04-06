using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AcmeGlobalCollege.Models.Entities
{
    public class FacultyProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string FacultyNumber { get; set; } = string.Empty;

        [Required]
        public string IdentityUserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [Required]
        public int BranchId { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? Specialization { get; set; }

        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        
        public virtual IdentityUser? IdentityUser { get; set; }
        public virtual Branch? Branch { get; set; }
        public virtual ICollection<CourseFaculty> CourseFaculties { get; set; } = new List<CourseFaculty>();
    }
}