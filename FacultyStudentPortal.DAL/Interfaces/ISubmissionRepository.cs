using FacultyStudentPortal.Models.Entities;

namespace FacultyStudentPortal.DAL.Interfaces
{
    public interface ISubmissionRepository
    {
        Task AddAsync(Submission submission);
        Task<IEnumerable<Submission>> GetByAssignmentIdAsync(int assignmentId);
        Task<Submission> GetByStudentAndAssignmentAsync(int studentId, int assignmentId);
        Task<IEnumerable<Submission>> GetByStudentIdAsync(int studentId);

    }
}
