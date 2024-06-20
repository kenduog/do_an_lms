using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Microsoft.AspNetCore.Http;
using Request.DomainRequests;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface IStudentService : IDomainService<tbl_Student, StudentSearch>
    {
        Task<AppDomainResult> GetStudentsForArrange(ArrangeNewClassSearch request);
        Task<PagedList<tbl_Student>> AvailableStudent(AvailableStudentRequest request);
        Task<List<StudentSelection>> GetByParent(Guid parentId);
        Task<PagedList<tbl_Student>> GetStudentByGrade(GetStudentByGradeRequest request);
        Task<SchoolReportCardModel> SchoolReportCard(Guid studentId);
        Task<string> Export(StudentSearchExport baseSearch);
        Task<List<ItemStudentImport>> Import(IFormFile excelFile);
        Task<string> ExportTemplate();
        Task<tbl_Student> UpdateStatus(Guid? studentId, int status);
    }
}
