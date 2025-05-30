using FacultyStudentPortal.Models.Entities;

namespace FacultyStudentPortal.DAL.Interfaces
{
    public interface IGradeRepository
    {
        Task AddAsync(Grade grade);
        Task<IEnumerable<Grade>> GetByStudentIdAsync(int studentId);
    }
}
