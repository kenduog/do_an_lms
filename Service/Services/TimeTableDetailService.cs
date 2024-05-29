using AutoMapper;
using Entities;
using Entities.AuthEntities;
using Entities.DataTransferObject;
using Entities.Search;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing.Theme;
using Request.RequestCreate;
using Request.RequestUpdate;
using Service.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Utilities;

namespace Service.Services
{
    public class TimeTableDetailService : DomainService<tbl_TimeTableDetail, TimeTableDetailSearch>, ITimeTableDetailService
    {
        private readonly IAppDbContext appDbContext;
        private readonly ISendNotificationService sendNotificationService;

        public TimeTableDetailService(IServiceProvider serviceProvider, IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext app) : base(unitOfWork, mapper)
        {
            this.appDbContext = app;
            this.sendNotificationService = serviceProvider.GetRequiredService<ISendNotificationService>();
        }

        protected override string GetStoreProcName()
        {
            return "Get_TimeTableDetail";
        }

        public override async Task Validate(tbl_TimeTableDetail model)
        {
            if (model.id != Guid.Empty)
            {
                var item = await this.GetByIdAsync(model.id) ?? throw new AppException(MessageContants.nf_item);
                model.timeTableId = item.timeTableId;
                model.classId = item.classId;
                model.subjectId = model.subjectId ?? item.subjectId;
                model.teacherId = model.teacherId ?? item.teacherId;
            }

            var timeTable = await this.unitOfWork.Repository<tbl_TimeTable>().GetQueryable().FirstOrDefaultAsync(x => x.deleted == false && x.id == model.timeTableId)
                    ?? throw new AppException(MessageContants.nf_timeTable);
            var _class = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().FirstOrDefaultAsync(x => x.deleted == false && x.id == model.classId)
                ?? throw new AppException(MessageContants.nf_class);

            var subject = await this.unitOfWork.Repository<tbl_Subject>().GetQueryable().FirstOrDefaultAsync(x => x.deleted == false && x.id == model.subjectId)
                   ?? throw new AppException(MessageContants.nf_subject);
            var teacher = await this.unitOfWork.Repository<tbl_Staff>().GetQueryable().FirstOrDefaultAsync(x => x.deleted == false && x.id == model.teacherId)
                ?? throw new AppException(MessageContants.nf_teacher);

            //check trùng với tkb hiện tại
            var timeTableDetail = await this.unitOfWork.Repository<tbl_TimeTableDetail>().GetQueryable()
                .AnyAsync(x =>
            x.deleted == false && x.timeTableId == model.timeTableId
            && ((x.sTime <= model.sTime && model.sTime <= x.eTime) || (x.sTime <= model.eTime && model.eTime <= x.eTime))
            && x.day == model.day
            && x.id != model.id
            && x.teacherId == model.teacherId);
            if (timeTableDetail)
                throw new AppException(MessageContants.exs_scheduleTeacher);

            // kiểm tra ca học mẫu giáo
            var classShift = await this.unitOfWork.Repository<tbl_ClassShift>().GetQueryable().Where(x => x.classId == _class.id && x.deleted == false).ToListAsync();
            if (classShift.Any())
            {
                var shift = classShift.Where(x => x.day == model.day).ToList();
                if (shift.Any())
                {
                    var checkTime = shift.FirstOrDefault(x => x.sTime <= model.sTime && x.eTime >= model.eTime)
                    ?? throw new AppException(MessageContants.time_classshift_error);
                }
                else
                {
                    throw new AppException(MessageContants.time_classshift_error);
                }
            }
            // Kiểm tra ngày học
            var dayOfWeek = await this.unitOfWork.Repository<tbl_DayOfWeek>().GetQueryable().FirstOrDefaultAsync(x => x.deleted == false
            && x.key == model.day
            && x.active == true) ?? throw new AppException(MessageContants.today_day_of_week_not_timetable);

            //validate teacher have another schedule in time
            //kiểm tra xem giờ này giáo viên có đứng lớp buổi nào không, với giáo viên thường
            //nếu giáo viên là giáo viên chủ nhiệm nhưng giờ này lớp của giáo viên đó đang học giáo viên khác thì được phép dạy
            string stringParams = this.GenerateParamsString(new { _class.schoolYearId, _class.branchId, model.teacherId, model.day });
            var schedules = await this.appDbContext.Set<tbl_TimeTableDetail>().FromSqlRaw($"Get_TeacherSchedule {stringParams}").ToListAsync();
            schedules = schedules.Where(x => x.id != model.id).ToList();

            if (schedules.Any())
            {
                //nếu bị trùng lịch thì throw new Appexception
                var exsist = schedules.Any(x => (x.sTime <= model.sTime && model.sTime <= x.eTime) || (x.sTime <= model.eTime && model.eTime <= x.eTime));
                if (exsist)
                    throw new AppException(MessageContants.exs_scheduleTeacher);

                //Ngược lại thì kiểm tra xem những lớp thằng này chủ nhiệm, nếu kh có lớp nào thì nó kh phải chủ nhiệm => pass
                var formClasses = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().Where(x => x.deleted == false && x.teacherId == teacher.id && x.schoolYearId == model.schoolYearId && x.branchId == model.branchId).ToListAsync();
                if (formClasses.Any())
                {
                    //nếu có lớp dc chủ nhiệm: check những giờ kh học của lớp, nếu trùng ngay những giờ này thì cũng coi như là trùng

                }
            }

            //check các lịch thủ công trong tương lai của tkb này có cái nào bị trùng không, nếu trùng thì báo luôn tên lớp với tuần bị trùng
            var dupSchedules = await this.unitOfWork.Repository<tbl_Schedule>().GetQueryable()
             .FirstOrDefaultAsync(x => x.deleted == false
                 && x.timeTableId == model.timeTableId
                 && ((long)x.sTime) % 86400000 <= model.eTime
                 && ((long)x.eTime) % 86400000 >= model.sTime
                 && x.sTime >= Timestamp.Now() //chỉ validate với các lịch trong tương lai
                 && x.type == 2
                 );
            if (dupSchedules != null)
            {
                string message = $"Tiết học đã bị trùng với lịch học bù của lớp {_class.name} vào lúc {Timestamp.ToString(dupSchedules.sTime, "HH:mm dd/MM/yyyy")}. Vui lòng kiểm tra lại!";
                throw new AppException(message);
            }

            //pass hết thì dc thêm/chỉnh sửa tiết
        }
        public override async Task UpdateItem(tbl_TimeTableDetail model)
        {
            await Validate(model);
            await UpdateAsync(model);

            // Gửi thông báo cho giáo viên khi có lịch thay đổi
            var teacher = await this.unitOfWork.Repository<tbl_Staff>().GetQueryable().FirstOrDefaultAsync(
                x => x.id == model.teacherId && x.deleted == false);
            var classes = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().FirstOrDefaultAsync(
                x => x.id == model.classId && x.deleted == false);
            if (teacher != null && classes != null)
            {
                var user = await this.unitOfWork.Repository<tbl_Users>().GetQueryable().FirstOrDefaultAsync(
                      x => x.id == teacher.userId && x.deleted == false);
                if (user != null)
                {
                    List<tbl_Users> receivers = new List<tbl_Users>();
                    receivers.Add(user);
                    List<IDictionary<string, string>> notiParamList = new List<IDictionary<string, string>>();
                    IDictionary<string, string> notiParam = new Dictionary<string, string>();
                    notiParam.Add("[ClassName]", classes.name);
                    notiParamList.Add(notiParam);
                    string subLink = "/preschool-class/class/detail/";
                    string linkQuery = "classId=" + classes.id.ToString();
                    sendNotificationService.SendNotification(Guid.Parse("4ea4997b-97e3-4c42-9292-dbbbb8015b35"), receivers, notiParamList, null, linkQuery, null, LookupConstant.ScreenCode_Class, subLink);
                }
            }
        }
        public override async Task AddItem(tbl_TimeTableDetail model)
        {
            await Validate(model);
            await CreateAsync(model);

            // Gửi thông báo cho giáo viên khi được chỉ định dạy
            var teacher = await this.unitOfWork.Repository<tbl_Staff>().GetQueryable().FirstOrDefaultAsync(
                x => x.id == model.teacherId && x.deleted == false);
            var classes = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().FirstOrDefaultAsync(
                x => x.id == model.classId && x.deleted == false);
            if (teacher != null && classes != null)
            {
                var user = await this.unitOfWork.Repository<tbl_Users>().GetQueryable().FirstOrDefaultAsync(
                      x => x.id == teacher.userId && x.deleted == false);
                if (user != null)
                {
                    List<tbl_Users> receivers = new List<tbl_Users>();
                    receivers.Add(user);
                    List<IDictionary<string, string>> notiParamList = new List<IDictionary<string, string>>();
                    IDictionary<string, string> notiParam = new Dictionary<string, string>();
                    notiParam.Add("[ClassName]", classes.name);
                    notiParamList.Add(notiParam);
                    string subLink = "/preschool-class/class/detail/";
                    string linkQuery = "classId=" + classes.id.ToString();
                    sendNotificationService.SendNotification(Guid.Parse("9ffd37d0-ecd9-4cf9-3b0f-08dc3551e232"), receivers, notiParamList, null, linkQuery, null, LookupConstant.ScreenCode_Class, subLink);
                }
            }
        }
    }
}
