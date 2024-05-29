using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Interface.Services
{
    public interface IStudentInClassService : IDomainService<tbl_StudentInClass, StudentInClassSearch>
    {
        Task<List<UserOption>> GetStudentAvailable(StudentAvailableSearch baseSearch);
        Task<string> GetClassNameByStudent(Guid studentId);
        Task<Guid> GetClassIdByStudent(Guid studentId);
        Task SendNotification(tbl_Student student, tbl_Class classes);
        Task<List<tbl_StudentInClass>> GetClassOfStudent(Guid studentId, List<tbl_StudentInClass> allClasses, tbl_Student student);
    }
}
