using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using AcmeGlobalCollege.Data;
using AcmeGlobalCollege.Models.Entities;
using AcmeGlobalCollege.Services.Implementations;
using System;
using System.Threading.Tasks;

namespace AcmeGlobalCollege.Tests.Services
{
    public class StudentServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly StudentService _studentService;

        public StudentServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _studentService = new StudentService(_context);

            
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            
            var branch = new Branch
            {
                Id = 1,
                Name = "Test Branch",
                Address = "Test Address"
            };
            _context.Branches.Add(branch);

            var student = new StudentProfile
            {
                Id = 1,
                StudentNumber = "TEST001",
                Name = "Test Student",
                Email = "test@test.com",
                IdentityUserId = "user123",  
                BranchId = 1,
                Status = "Active",
                DateOfBirth = new DateTime(2000, 1, 1),
                EnrolmentDate = DateTime.UtcNow
            };
            _context.StudentProfiles.Add(student);

            _context.SaveChanges();
        }

        [Fact]
        public async Task GetStudentByIdAsync_ShouldReturnStudent_WhenExists()
        {
            
            var result = await _studentService.GetStudentByIdAsync(1);

            
            Assert.NotNull(result);
            Assert.Equal("TEST001", result.StudentNumber);
            Assert.Equal("Test Student", result.Name);
        }

        [Fact]
        public async Task GetStudentByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            
            var result = await _studentService.GetStudentByIdAsync(999);

           
            Assert.Null(result);
        }

        [Fact]
        public async Task GetStudentByUserIdAsync_ShouldReturnCorrectStudent()
        {
            
            var result = await _studentService.GetStudentByUserIdAsync("user123");

           
            Assert.NotNull(result);  
            Assert.Equal("Test Student", result.Name);
        }

        [Fact]
        public async Task GetAllStudentsAsync_ShouldReturnAllStudents()
        {
            
            var results = await _studentService.GetAllStudentsAsync();

          
            Assert.Single(results);  
        }

        [Fact]
        public async Task CreateStudentAsync_ShouldAddStudentToDatabase()
        {
           
            var branch = new Branch { Id = 2, Name = "Another Branch", Address = "Address" };
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            var newStudent = new StudentProfile
            {
                Name = "New Student",
                Email = "new@test.com",
                IdentityUserId = "user456",
                BranchId = 2,
                DateOfBirth = new DateTime(2000, 1, 1)
            };

            
            var result = await _studentService.CreateStudentAsync(newStudent);

           
            Assert.NotNull(result);
            Assert.NotNull(result.StudentNumber);
            Assert.Contains("STU-", result.StudentNumber);
            Assert.Equal(2, await _context.StudentProfiles.CountAsync());
        }

        [Fact]
        public async Task UpdateStudentAsync_ShouldUpdateStudentDetails()
        {
            
            var student = await _studentService.GetStudentByIdAsync(1);
            student.Name = "Updated Name";

            // Act
            await _studentService.UpdateStudentAsync(student);
            var updated = await _studentService.GetStudentByIdAsync(1);

            // Assert
            Assert.Equal("Updated Name", updated.Name);
        }

        [Fact]
        public async Task DeleteStudentAsync_ShouldRemoveStudent()
        {
            
            var branch = new Branch { Id = 3, Name = "Delete Branch", Address = "Address" };
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            var studentToDelete = new StudentProfile
            {
                StudentNumber = "DELETE001",
                Name = "To Be Deleted",
                Email = "delete@test.com",
                IdentityUserId = "userDelete",
                BranchId = 3,
                DateOfBirth = new DateTime(2000, 1, 1),
                Status = "Active"
            };

            await _studentService.CreateStudentAsync(studentToDelete);
            var studentId = studentToDelete.Id;

           
            Assert.Equal(2, await _context.StudentProfiles.CountAsync()); 

           
            await _studentService.DeleteStudentAsync(studentId);

            
            Assert.Equal(1, await _context.StudentProfiles.CountAsync()); 
        }

        [Fact]
        public async Task GetStudentEnrolmentsAsync_ShouldReturnEnrolments()
        {
            
            var enrolment = new CourseEnrolment
            {
                StudentProfileId = 1,
                CourseId = 1,
                Status = "Enrolled"
            };
            _context.CourseEnrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            
            var enrolments = await _studentService.GetStudentEnrolmentsAsync(1);

           
            Assert.Single(enrolments);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}