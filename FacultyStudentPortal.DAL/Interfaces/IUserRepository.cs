using FacultyStudentPortal.Models.Entities;
using System.Threading.Tasks;

namespace FacultyStudentPortal.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllStudentsAsync();
        Task<User> AuthenticateAsync(string email, string passwordHash);
        Task<IEnumerable<User>> GetFacultyUsersAsync();

    }
}
