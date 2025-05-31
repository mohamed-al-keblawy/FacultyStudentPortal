using FacultyStudentPortal.DAL.Interfaces;
using FacultyStudentPortal.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacultyStudentPortal.BLL
{
    public class AssignmentService
    {
        private readonly IAssignmentRepository _repo;

        public AssignmentService(IAssignmentRepository repo)
        {
            _repo = repo;
        }
        public Task<IEnumerable<Assignment>> GetAllAsync() => _repo.GetAllAsync();
    }

}
