using FacultyStudentPortal.Models.Entities;

namespace FacultyStudentPortal.DAL.Interfaces
{
    public interface IAssignmentRepository
    {
        Task<IEnumerable<Assignment>> GetAllAsync();
        Task<Assignment> GetByIdAsync(int id);
        Task AddAsync(Assignment assignment);
        Task<IEnumerable<Assignment>> GetByStudentIdAsync(int studentId);
    }
}
