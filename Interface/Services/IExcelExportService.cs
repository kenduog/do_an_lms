using Entities;
using Entities.DataTransferObject;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface IExcelExportService 
    {
        string Export<T>(ExcelPayload<T> payload);
        List<T> FetchDataToModel<T>(IFormFile file, Dictionary<int, string> tokens, int fromRow) where T : new();
        Dictionary<int, string> InitTokenHeader(List<string> headers, int from);
        string ExportTemplate<T>(ExcelPayload<T> payload, List<ListDropDown> dropdown);
    }
}
