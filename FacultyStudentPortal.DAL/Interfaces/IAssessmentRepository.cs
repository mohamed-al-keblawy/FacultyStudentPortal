using FacultyStudentPortal.Models.Entities;

namespace FacultyStudentPortal.DAL.Interfaces
{
    public interface IAssessmentRepository
    {
        Task AddAsync(Assessment assessment);
        Task<IEnumerable<Assessment>> GetByAssignmentIdAsync(int assignmentId);
    }
}
