using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using AcmeGlobalCollege.Data;
using AcmeGlobalCollege.Models.Entities;
using AcmeGlobalCollege.Services.Implementations;
using AcmeGlobalCollege.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace AcmeGlobalCollege.Tests.Services
{
    public class GradebookServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IFacultyService> _mockFacultyService;
        private readonly GradebookService _gradebookService;

        public GradebookServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockFacultyService = new Mock<IFacultyService>();
            _gradebookService = new GradebookService(_context, _mockFacultyService.Object);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var course = new Course
            {
                Id = 1,
                Code = "TEST101",
                Name = "Test Course",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(3)
            };
            _context.Courses.Add(course);

            var assignment = new Assignment
            {
                Id = 1,
                CourseId = 1,
                Title = "Test Assignment",
                MaxScore = 100,
                DueDate = DateTime.UtcNow.AddDays(30)
            };
            _context.Assignments.Add(assignment);

            var exam = new Exam
            {
                Id = 1,
                CourseId = 1,
                Title = "Test Exam",
                MaxScore = 100,
                Date = DateTime.UtcNow.AddMonths(2),
                ResultsReleased = false
            };
            _context.Exams.Add(exam);

            var student = new StudentProfile
            {
                Id = 1,
                StudentNumber = "TEST001",
                Name = "Test Student",
                Email = "test@test.com"
            };
            _context.StudentProfiles.Add(student);

            _context.SaveChanges();
        }

        [Fact]
        public async Task GetCourseAssignmentsAsync_ShouldReturnAssignments()
        {
            
            var results = await _gradebookService.GetCourseAssignmentsAsync(1);

            
            Assert.Single(results);
            Assert.Equal("Test Assignment", results.First().Title);
        }

        [Fact]
        public async Task GetCourseExamsAsync_ShouldReturnExams()
        {
            
            var results = await _gradebookService.GetCourseExamsAsync(1);

            
            Assert.Single(results);
            Assert.Equal("Test Exam", results.First().Title);
        }

        [Fact]
        public async Task RecordAssignmentResultAsync_ShouldCreateNewResult()
        {
            
            var assignmentId = 1;
            var studentId = 1;
            var score = 85;
            var feedback = "Good job!";

            
            var result = await _gradebookService.RecordAssignmentResultAsync(assignmentId, studentId, score, feedback);

            
            Assert.NotNull(result);
            Assert.Equal(85, result.Score);
            Assert.Equal("Good job!", result.Feedback);
            Assert.Equal(1, await _context.AssignmentResults.CountAsync());
        }

        [Fact]
        public async Task RecordAssignmentResultAsync_ShouldUpdateExistingResult()
        {
            
            var existingResult = new AssignmentResult
            {
                AssignmentId = 1,
                StudentProfileId = 1,
                Score = 70,
                Feedback = "Initial feedback"
            };
            await _context.AssignmentResults.AddAsync(existingResult);
            await _context.SaveChangesAsync();

            
            var result = await _gradebookService.RecordAssignmentResultAsync(1, 1, 90, "Updated feedback");

            
            Assert.Equal(90, result.Score);
            Assert.Equal("Updated feedback", result.Feedback);
            Assert.Single(await _context.AssignmentResults.ToListAsync());
        }

        [Fact]
        public async Task RecordAssignmentResultAsync_ShouldCapScoreAtMaxScore()
        {
            
            var result = await _gradebookService.RecordAssignmentResultAsync(1, 1, 150, "Over max");

            
            Assert.Equal(100, result.Score); // Max score is 100
        }

        [Fact]
        public async Task RecordExamResultAsync_ShouldCalculateGradeCorrectly()
        {
            
            var result = await _gradebookService.RecordExamResultAsync(1, 1, 85);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(85, result.Score);
            Assert.Equal("B", result.Grade);
        }

        [Theory]
        [InlineData(95, "A")]
        [InlineData(85, "B")]
        [InlineData(75, "C")]
        [InlineData(65, "D")]
        [InlineData(55, "F")]
        public async Task RecordExamResultAsync_ShouldAssignCorrectGrade(int score, string expectedGrade)
        {
            
            var result = await _gradebookService.RecordExamResultAsync(1, 1, score);

            
            Assert.Equal(expectedGrade, result.Grade);
        }

        [Fact]
        public async Task CalculateStudentOverallGradeAsync_ShouldCalculateCorrectPercentage()
        {
            
            var assignmentResult = new AssignmentResult
            {
                AssignmentId = 1,
                StudentProfileId = 1,
                Score = 80
            };
            await _context.AssignmentResults.AddAsync(assignmentResult);

            var examResult = new ExamResult
            {
                ExamId = 1,
                StudentProfileId = 1,
                Score = 90,
                Grade = "A"
            };
            await _context.ExamResults.AddAsync(examResult);
            await _context.SaveChangesAsync();

            
            var grade = await _gradebookService.CalculateStudentOverallGradeAsync(1, 1);

            
            Assert.Equal(85m, grade);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}