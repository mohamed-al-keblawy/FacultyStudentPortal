using Dapper;
using FacultyStudentPortal.DAL.Database;
using FacultyStudentPortal.DAL.Interfaces;
using FacultyStudentPortal.Models.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FacultyStudentPortal.DAL.Repositories
{
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public SubmissionRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(Submission submission)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("sp_InsertSubmission", new
            {
                submission.AssignmentId,
                submission.StudentId,
                submission.SubmittedAt,
                submission.FilePath
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Submission>> GetByAssignmentIdAsync(int assignmentId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Submission>(
                "sp_GetSubmissionsByAssignment", new { AssignmentId = assignmentId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<Submission> GetByStudentAndAssignmentAsync(int studentId, int assignmentId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Submission>(
                "sp_GetSubmissionByStudent", new { StudentId = studentId, AssignmentId = assignmentId }, commandType: CommandType.StoredProcedure);
        }
    }
}
