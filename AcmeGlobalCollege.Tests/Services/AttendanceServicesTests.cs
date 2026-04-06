using Xunit;
using Microsoft.EntityFrameworkCore;
using AcmeGlobalCollege.Data;
using AcmeGlobalCollege.Models.Entities;
using AcmeGlobalCollege.Services.Implementations;
using System;
using System.Threading.Tasks;

namespace AcmeGlobalCollege.Tests.Services
{
    public class AttendanceServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly AttendanceService _attendanceService;

        public AttendanceServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _attendanceService = new AttendanceService(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var course = new Course { Id = 1, Code = "TEST101", Name = "Test Course" };
            _context.Courses.Add(course);

            var student = new StudentProfile { Id = 1, StudentNumber = "TEST001", Name = "Test Student" };
            _context.StudentProfiles.Add(student);

            var enrolment = new CourseEnrolment
            {
                Id = 1,
                StudentProfileId = 1,
                CourseId = 1,
                Status = "Enrolled"
            };
            _context.CourseEnrolments.Add(enrolment);

            _context.SaveChanges();
        }

        [Fact]
        public async Task RecordAttendanceAsync_ShouldCreateNewRecord()
        {
            
            var result = await _attendanceService.RecordAttendanceAsync(1, 1, DateTime.Today, true, "Good attendance");

            
            Assert.NotNull(result);
            Assert.True(result.IsPresent);
            Assert.Equal("Good attendance", result.Notes);
            Assert.Equal(1, await _context.AttendanceRecords.CountAsync());
        }

        [Fact]
        public async Task RecordAttendanceAsync_ShouldUpdateExistingRecord()
        {
            
            var existing = new AttendanceRecord
            {
                CourseEnrolmentId = 1,
                WeekNumber = 1,
                Date = DateTime.Today,
                IsPresent = true
            };
            await _context.AttendanceRecords.AddAsync(existing);
            await _context.SaveChangesAsync();

            
            var result = await _attendanceService.RecordAttendanceAsync(1, 1, DateTime.Today, false, "Absent this week");

           
            Assert.False(result.IsPresent);
            Assert.Equal("Absent this week", result.Notes);
            Assert.Single(await _context.AttendanceRecords.ToListAsync());
        }

        [Fact]
        public async Task GetAttendanceByCourseAsync_ShouldReturnCourseAttendance()
        {
            
            for (int i = 1; i <= 3; i++)
            {
                await _attendanceService.RecordAttendanceAsync(1, i, DateTime.Today.AddDays(-i), i % 2 == 0, null);
            }

            
            var results = await _attendanceService.GetAttendanceByCourseAsync(1, 2);

            
            Assert.Single(results);
        }

        [Fact]
        public async Task GetCourseAttendancePercentageAsync_ShouldCalculateCorrectPercentage()
        {
            
            await _attendanceService.RecordAttendanceAsync(1, 1, DateTime.Today.AddDays(-7), true, null);
            await _attendanceService.RecordAttendanceAsync(1, 2, DateTime.Today.AddDays(-6), true, null);
            await _attendanceService.RecordAttendanceAsync(1, 3, DateTime.Today.AddDays(-5), false, null);
            await _attendanceService.RecordAttendanceAsync(1, 4, DateTime.Today.AddDays(-4), true, null);

            
            var percentage = await _attendanceService.GetCourseAttendancePercentageAsync(1);

            
            Assert.Equal(75m, percentage); 
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}