using Dapper;
using FacultyStudentPortal.DAL.Database;
using FacultyStudentPortal.DAL.Interfaces;
using FacultyStudentPortal.Models.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FacultyStudentPortal.DAL.Repositories
{
    public class AssessmentRepository : IAssessmentRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public AssessmentRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(Assessment assessment)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("sp_InsertAssessment", new
            {
                assessment.AssignmentId,
                assessment.Criterion,
                assessment.MaxScore
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Assessment>> GetByAssignmentIdAsync(int assignmentId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Assessment>(
                "sp_GetAssessmentsByAssignment", new { AssignmentId = assignmentId }, commandType: CommandType.StoredProcedure);
        }
    }
}
