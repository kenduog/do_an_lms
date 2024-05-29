using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface IScaleMeasureDetailService : IDomainService<tbl_ScaleMeasureDetail, ScaleMeasureDetailSearch>
    {
        Task<string> ExportStudentList(Guid scaleMeasureId);
        Task<List<ScaleMeasureDetailExportWithClient>> Import(IFormFile excelFile);
        Task<List<StudentScaleMeasureDetail>> GetMobileScaleMeasure(Guid parentId);
        Task<string> ExportTemplate(ScaleMeasureDetailExportSearch baseSearch);
        Task<string> Export(ScaleMeasureDetailExportSearch baseSearch);
        Task SaveList(List<ScaleMeasureDetailExport> datas, Guid? scaleMeasureId);
    }
}
