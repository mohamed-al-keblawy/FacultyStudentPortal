using FacultyStudentPortal.BLL;
using FacultyStudentPortal.DAL.Interfaces;
using FacultyStudentPortal.Models.Entities;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FacultyStudentPortal.Tests.Services
{
    public class AssignmentServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsAssignments()
        {
            // Arrange
            var mockRepo = new Mock<IAssignmentRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Assignment> {
                new Assignment { AssignmentId = 1, Title = "Test Assignment" }
            });

            var service = new AssignmentService(mockRepo.Object);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Assignment", result.First().Title);
        }
    }
}
