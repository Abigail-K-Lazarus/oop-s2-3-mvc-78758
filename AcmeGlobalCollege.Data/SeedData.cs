using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using AcmeGlobalCollege.Models.Entities;

namespace AcmeGlobalCollege.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            
            await context.Database.EnsureCreatedAsync();

            
            string[] roles = { "Admin", "Faculty", "Student" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            
            if (!context.Branches.Any())
            {
                var branches = new[]
                {
                    new Branch { Name = "Dublin", Address = "123 Main Street, Dublin 1", Phone = "01-2345678", Email = "dublin@acmeglobal.ie" },
                    new Branch { Name = "Cork", Address = "456 Patrick Street, Cork", Phone = "021-2345678", Email = "cork@acmeglobal.ie" },
                    new Branch { Name = "Galway", Address = "789 Shop Street, Galway", Phone = "091-2345678", Email = "galway@acmeglobal.ie" }
                };
                await context.Branches.AddRangeAsync(branches);
                await context.SaveChangesAsync();
            }

            
            var adminEmail = "admin@acme.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            
            var facultyEmail = "faculty@acme.com";
            if (await userManager.FindByEmailAsync(facultyEmail) == null)
            {
                var faculty = new IdentityUser
                {
                    UserName = facultyEmail,
                    Email = facultyEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(faculty, "Faculty@123");
                await userManager.AddToRoleAsync(faculty, "Faculty");

                
                var dublinBranch = context.Branches.FirstOrDefault(b => b.Name == "Dublin");
                var facultyProfile = new FacultyProfile
                {
                    IdentityUserId = faculty.Id,
                    FacultyNumber = "FAC-2024-0001",
                    Name = "Dr. John Smith",
                    Email = facultyEmail,
                    Phone = "087-1234567",
                    BranchId = dublinBranch?.Id ?? 1,
                    Department = "Computer Science",
                    Specialization = "Web Development",
                    HireDate = DateTime.UtcNow.AddYears(-5)
                };
                await context.FacultyProfiles.AddAsync(facultyProfile);
                await context.SaveChangesAsync();
            }

           
            var studentEmail = "student@acme.com";
            if (await userManager.FindByEmailAsync(studentEmail) == null)
            {
                var student = new IdentityUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(student, "Student@123");
                await userManager.AddToRoleAsync(student, "Student");

                
                var dublinBranch = context.Branches.FirstOrDefault(b => b.Name == "Dublin");
                var studentProfile = new StudentProfile
                {
                    IdentityUserId = student.Id,
                    StudentNumber = "STU-2024-0001",
                    Name = "Jane Doe",
                    Email = studentEmail,
                    Phone = "085-1234567",
                    Address = "123 College Road, Dublin",
                    DateOfBirth = new DateTime(2000, 5, 15),
                    BranchId = dublinBranch?.Id ?? 1,
                    Status = "Active"
                };
                await context.StudentProfiles.AddAsync(studentProfile);
                await context.SaveChangesAsync();
            }

           
            if (!context.Courses.Any())
            {
                var dublinBranch = context.Branches.FirstOrDefault(b => b.Name == "Dublin");
                var corkBranch = context.Branches.FirstOrDefault(b => b.Name == "Cork");

                var courses = new[]
                {
                    new Course { Code = "CS101", Name = "Introduction to Programming", Description = "Basic programming concepts using C#", MaxCapacity = 30, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(4), BranchId = dublinBranch!.Id },
                    new Course { Code = "CS201", Name = "Web Development", Description = "HTML, CSS, JavaScript, ASP.NET Core", MaxCapacity = 25, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(4), BranchId = dublinBranch!.Id },
                    new Course { Code = "CS301", Name = "Database Design", Description = "SQL Server and Entity Framework", MaxCapacity = 28, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(4), BranchId = dublinBranch!.Id },
                    new Course { Code = "BUS101", Name = "Business Fundamentals", Description = "Introduction to business management", MaxCapacity = 35, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(4), BranchId = corkBranch!.Id }
                };
                await context.Courses.AddRangeAsync(courses);
                await context.SaveChangesAsync();
            }

           
            if (!context.CourseFaculties.Any())
            {
                var faculty = context.FacultyProfiles.FirstOrDefault();
                var courses = context.Courses.ToList();

                if (faculty != null && courses.Any())
                {
                    foreach (var course in courses.Take(2))
                    {
                        await context.CourseFaculties.AddAsync(new CourseFaculty
                        {
                            CourseId = course.Id,
                            FacultyProfileId = faculty.Id,
                            AssignedDate = DateTime.UtcNow,
                            IsPrimary = true
                        });
                    }
                    await context.SaveChangesAsync();
                }
            }

            
            if (!context.CourseEnrolments.Any())
            {
                var student = context.StudentProfiles.FirstOrDefault();
                var courses = context.Courses.Take(2).ToList();

                if (student != null && courses.Any())
                {
                    foreach (var course in courses)
                    {
                        await context.CourseEnrolments.AddAsync(new CourseEnrolment
                        {
                            StudentProfileId = student.Id,
                            CourseId = course.Id,
                            EnrolDate = DateTime.UtcNow,
                            Status = "Enrolled"
                        });
                    }
                    await context.SaveChangesAsync();
                }
            }

            
            if (!context.Assignments.Any())
            {
                var courses = context.Courses.ToList();
                var assignments = new List<Assignment>();

                foreach (var course in courses)
                {
                    assignments.Add(new Assignment
                    {
                        CourseId = course.Id,
                        Title = "Assignment 1",
                        Description = "First assignment of the semester",
                        MaxScore = 100,
                        DueDate = DateTime.UtcNow.AddMonths(1)
                    });

                    assignments.Add(new Assignment
                    {
                        CourseId = course.Id,
                        Title = "Assignment 2",
                        Description = "Second assignment",
                        MaxScore = 100,
                        DueDate = DateTime.UtcNow.AddMonths(2)
                    });
                }

                await context.Assignments.AddRangeAsync(assignments);
                await context.SaveChangesAsync();
            }

            
            if (!context.Exams.Any())
            {
                var courses = context.Courses.ToList();
                var exams = new List<Exam>();

                foreach (var course in courses)
                {
                    exams.Add(new Exam
                    {
                        CourseId = course.Id,
                        Title = "Midterm Exam",
                        Date = DateTime.UtcNow.AddMonths(2),
                        MaxScore = 100,
                        ResultsReleased = false
                    });

                    exams.Add(new Exam
                    {
                        CourseId = course.Id,
                        Title = "Final Exam",
                        Date = DateTime.UtcNow.AddMonths(4),
                        MaxScore = 100,
                        ResultsReleased = false
                    });
                }

                await context.Exams.AddRangeAsync(exams);
                await context.SaveChangesAsync();
            }

            
            if (!context.AssignmentResults.Any())
            {
                var student = context.StudentProfiles.FirstOrDefault();
                var assignments = context.Assignments.ToList();

                if (student != null && assignments.Any())
                {
                    var random = new Random();
                    foreach (var assignment in assignments.Take(2))
                    {
                        await context.AssignmentResults.AddAsync(new AssignmentResult
                        {
                            AssignmentId = assignment.Id,
                            StudentProfileId = student.Id,
                            Score = random.Next(65, 95),
                            Feedback = "Good work! Keep it up.",
                            SubmittedAt = DateTime.UtcNow
                        });
                    }
                    await context.SaveChangesAsync();
                }
            }

           
            if (!context.AttendanceRecords.Any())
            {
                var enrolments = context.CourseEnrolments.ToList();
                var random = new Random();

                foreach (var enrolment in enrolments)
                {
                    for (int week = 1; week <= 8; week++)
                    {
                        await context.AttendanceRecords.AddAsync(new AttendanceRecord
                        {
                            CourseEnrolmentId = enrolment.Id,
                            WeekNumber = week,
                            Date = DateTime.UtcNow.AddDays(-(8 - week) * 7),
                            IsPresent = random.Next(0, 100) > 15 
                        });
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }
}