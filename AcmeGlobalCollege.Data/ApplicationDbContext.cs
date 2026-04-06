using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AcmeGlobalCollege.Models.Entities;

namespace AcmeGlobalCollege.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<FacultyProfile> FacultyProfiles { get; set; }
        public DbSet<CourseEnrolment> CourseEnrolments { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentResult> AssignmentResults { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<CourseFaculty> CourseFaculties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            // UNIQUE CONSTRAINTS
            // ========================
            modelBuilder.Entity<StudentProfile>()
                .HasIndex(s => s.StudentNumber)
                .IsUnique();

            modelBuilder.Entity<StudentProfile>()
                .HasIndex(s => s.IdentityUserId)
                .IsUnique();

            modelBuilder.Entity<FacultyProfile>()
                .HasIndex(f => f.FacultyNumber)
                .IsUnique();

            modelBuilder.Entity<FacultyProfile>()
                .HasIndex(f => f.IdentityUserId)
                .IsUnique();

            modelBuilder.Entity<CourseEnrolment>()
                .HasIndex(e => new { e.StudentProfileId, e.CourseId })
                .IsUnique();

            modelBuilder.Entity<CourseFaculty>()
                .HasIndex(cf => new { cf.CourseId, cf.FacultyProfileId })
                .IsUnique();

            
            modelBuilder.Entity<CourseFaculty>()
                .HasKey(cf => new { cf.CourseId, cf.FacultyProfileId });

            

            modelBuilder.Entity<CourseEnrolment>()
                .HasOne(ce => ce.StudentProfile)
                .WithMany(s => s.Enrolments)
                .HasForeignKey(ce => ce.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseEnrolment>()
                .HasOne(ce => ce.Course)
                .WithMany(c => c.Enrolments)
                .HasForeignKey(ce => ce.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AttendanceRecord>()
                .HasOne(ar => ar.CourseEnrolment)
                .WithMany(ce => ce.AttendanceRecords)
                .HasForeignKey(ar => ar.CourseEnrolmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssignmentResult>()
                .HasOne(ar => ar.Assignment)
                .WithMany(a => a.AssignmentResults)
                .HasForeignKey(ar => ar.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssignmentResult>()
                .HasOne(ar => ar.StudentProfile)
                .WithMany(s => s.AssignmentResults)
                .HasForeignKey(ar => ar.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Exams)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.Exam)
                .WithMany(e => e.ExamResults)
                .HasForeignKey(er => er.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.StudentProfile)
                .WithMany(s => s.ExamResults)
                .HasForeignKey(er => er.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseFaculty>()
                .HasOne(cf => cf.Course)
                .WithMany(c => c.CourseFaculties)
                .HasForeignKey(cf => cf.CourseId);

            modelBuilder.Entity<CourseFaculty>()
                .HasOne(cf => cf.FacultyProfile)
                .WithMany(f => f.CourseFaculties)
                .HasForeignKey(cf => cf.FacultyProfileId);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Branch)
                .WithMany(b => b.Courses)
                .HasForeignKey(c => c.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentProfile>()
                .HasOne(s => s.Branch)
                .WithMany(b => b.Students)
                .HasForeignKey(s => s.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FacultyProfile>()
                .HasOne(f => f.Branch)
                .WithMany(b => b.Faculties)
                .HasForeignKey(f => f.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentProfile>()
                .Property(s => s.Status)
                .HasDefaultValue("Active");

            modelBuilder.Entity<CourseEnrolment>()
                .Property(ce => ce.Status)
                .HasDefaultValue("Enrolled");

            modelBuilder.Entity<CourseEnrolment>()
                .Property(ce => ce.EnrolDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<AttendanceRecord>()
                .Property(ar => ar.Date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<AssignmentResult>()
                .Property(ar => ar.SubmittedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<ExamResult>()
                .Property(er => er.SubmittedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
