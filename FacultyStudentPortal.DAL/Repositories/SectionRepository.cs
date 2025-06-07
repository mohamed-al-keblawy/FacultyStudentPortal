using Dapper;
using FacultyStudentPortal.DAL.Database;
using FacultyStudentPortal.DAL.Interfaces;
using FacultyStudentPortal.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacultyStudentPortal.DAL.Repositories
{
    public class SectionRepository : ISectionRepository
    {
        private readonly DbConnectionFactory _factory;

        public SectionRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<Section>> GetAllAsync()
        {
            using var connection = _factory.CreateConnection();
            return await connection.QueryAsync<Section>(
                "sp_GetAllSections",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task AddAsync(Section section)
        {
            using var connection = _factory.CreateConnection();
            var sql = @"
            INSERT INTO Sections (CourseId, FacultyId, RoomId, DayOfWeek, StartTime, EndTime)
            VALUES (@CourseId, @FacultyId, @RoomId, @DayOfWeek, @StartTime, @EndTime)";

            await connection.ExecuteAsync(sql, section);
        }
    }

}
