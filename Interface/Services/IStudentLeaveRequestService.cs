using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Interface.Services
{
    public interface IStudentLeaveRequestService : IDomainService<tbl_StudentLeaveRequest, StudentLeaveRequestSearch>
    {
        Task AddRange(MultipleStudentLeaveRequest request);
    }
}
