using Dapper;
using FacultyStudentPortal.DAL.Database;
using FacultyStudentPortal.DAL.Interfaces;
using FacultyStudentPortal.Models.Entities;
using System.Data;
using System.Threading.Tasks;

namespace FacultyStudentPortal.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public UserRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(
                "sp_GetUserByEmail", new { Email = email }, commandType: CommandType.StoredProcedure);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(
                "sp_GetUserById", new { UserId = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<User>> GetAllStudentsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<User>(
                "sp_GetAllStudents", commandType: CommandType.StoredProcedure);
        }

        public async Task<User> AuthenticateAsync(string email, string passwordHash)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(
                "sp_AuthenticateUser", new { Email = email, PasswordHash = passwordHash }, commandType: CommandType.StoredProcedure);
        }

    }
}
