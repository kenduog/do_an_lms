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
    public interface ITimeTableDetailService : IDomainService<tbl_TimeTableDetail, TimeTableDetailSearch>
    {
    }
}
