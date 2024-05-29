using Entities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.RequestCreate;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.DataTransferObject;
using Request.RequestUpdate;
using System;

namespace Interface.Services
{
    public interface ITimeTableService : IDomainService<tbl_TimeTable, TimeTableSearch>
    {
        #region code a Hung
        //Task ValidateForUpdate(tbl_TimeTable entity);
        //Task<List<GenerateTimeTableDTO>> GenerateTimeTable(GenerateTimeTableCreate entity);

        #endregion
        Task<TimeTableResponse> CreateTimeTable(TimeTableCreate model);
        Task<TimeTableResponse> GenerateTimetable(Guid id);
    }
}
