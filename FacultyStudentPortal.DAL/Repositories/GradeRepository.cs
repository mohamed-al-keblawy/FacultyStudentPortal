using Dapper;
using FacultyStudentPortal.DAL.Database;
using FacultyStudentPortal.DAL.Interfaces;
using FacultyStudentPortal.Models.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FacultyStudentPortal.DAL.Repositories
{
    public class GradeRepository : IGradeRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public GradeRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(Grade grade)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("sp_InsertGrade", new
            {
                grade.AssessmentId,
                grade.StudentId,
                grade.Score,
                grade.Remarks
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Grade>> GetByStudentIdAsync(int studentId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Grade>(
                "sp_GetGradesByStudent", new { StudentId = studentId }, commandType: CommandType.StoredProcedure);
        }
    }
}
