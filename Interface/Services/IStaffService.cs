using Entities;
using Entities.AuthEntities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface IStaffService : IDomainService<tbl_Staff, StaffSearch>
    {
        Task<List<SubjectStaff>> GetSubjectByStaffId(Guid userId);
        Task<string> Export(StaffSearch baseSearch);
        Task<string> ExportTeacher(StaffSearch baseSearch);
        Task<List<ItemTeacherImport>> ImportTecher(IFormFile file);
        Task<List<ItemStaffImport>> ImportStaff(IFormFile file);
        Task<string> ExportStaffTemplate();
        Task<string> ExportTeacherTemplate();
        Task<Guid> GetDepartmentIdByName(string name);
        Task<Guid> GetGroupIdByName(string name);
        Task<bool> GetConditionValueByGroupName(string s);
    }
}
