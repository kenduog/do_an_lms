using Entities;
using Entities.AuthEntities;
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
    public interface IScheduleService : IDomainService<tbl_Schedule, BaseSearch>
    {
        Task<List<tbl_Schedule>> GetOrGenerateSchedule(ScheduleSearch request);
        Task<List<tbl_Schedule>> DailyActivity(StudentDailyActivityRequest request);
        Task SendScheduleNotification(ScheduleNotificationRequest request);
        Task<tbl_TimeTable> GetTimeTable(Guid classId);
        Task SendNotify(tbl_Class classes, List<tbl_Users> receivers);
    }
}
