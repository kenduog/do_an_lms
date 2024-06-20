using Entities;
using Entities.DataTransferObject;
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
    public interface IAttendanceService : IDomainService<tbl_Attendance, BaseSearch>
    {
        Task<AttendanceResponse> GetOrGenerateAttendance(AttendanceSearch request);
        Task SendNotification(AttendanceNotificationRequest request);
        Task GenerateAttendance(tbl_Class _class, Guid studentId);
    }
}
