using Dapper;
using FacultyStudentPortal.DAL.Database;
using FacultyStudentPortal.DAL.Interfaces;
using FacultyStudentPortal.Models.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FacultyStudentPortal.DAL.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public AssignmentRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Assignment>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Assignment>("sp_GetAllAssignments", commandType: CommandType.StoredProcedure);
        }

        public async Task<Assignment> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Assignment>(
                "sp_GetAssignmentById", new { AssignmentId = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task AddAsync(Assignment assignment)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("sp_InsertAssignment", new
            {
                assignment.Title,
                assignment.Description,
                assignment.DueDate,
                assignment.FilePath,
                assignment.CreatedBy
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Assignment>> GetByStudentIdAsync(int studentId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Assignment>(
                "sp_GetAssignmentsForStudent", new { StudentId = studentId }, commandType: CommandType.StoredProcedure);
        }
    }
}
