using Entities;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Service.Services.DomainServices;
using Entities.Search;
using Newtonsoft.Json;
using Entities.DomainEntities;
using System.Net;
using Request.RequestCreate;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Azure.Core;

namespace Service.Services
{
    public class AttendanceService : DomainService<tbl_Attendance, BaseSearch>, IAttendanceService
    {
        private readonly IParentService parentService;
        private readonly ISendNotificationService sendNotificationService;
        public AttendanceService(IAppUnitOfWork unitOfWork,
            IMapper mapper,
            IParentService parentService,
            ISendNotificationService sendNotificationService) : base(unitOfWork, mapper)
        {
            this.parentService = parentService;
            this.sendNotificationService = sendNotificationService;
        }
        protected override string GetStoreProcName()
        {
            return "Get_Attendance";
        }

        public override async Task Validate(tbl_Attendance model)
        {
            if (model.schoolYearId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_SchoolYear>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == model.schoolYearId)
                    ?? throw new AppException(MessageContants.nf_schoolYear);
            }
            if (model.branchId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Branch>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == model.branchId)
                    ?? throw new AppException(MessageContants.nf_branch);
            }
            if (model.classId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Class>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == model.classId)
                    ?? throw new AppException(MessageContants.nf_class);
            }
            if (model.studentId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Student>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == model.studentId)
                    ?? throw new AppException(MessageContants.nf_student);
            }
        }


        public async Task<AttendanceResponse> GetOrGenerateAttendance(AttendanceSearch request)
        {
            var _class = await this.unitOfWork.Repository<tbl_Class>().GetQueryable()
                .FirstOrDefaultAsync(x => x.id == (request.classId ?? Guid.Empty)) ?? throw new AppException(MessageContants.nf_class);
            List<tbl_Attendance> attendances = new List<tbl_Attendance>();
            AttendanceResponse result = new AttendanceResponse();

            //lấy dữ liệu điểm danh của theo request
            attendances = this.unitOfWork.Repository<tbl_Attendance>().ExcuteStoreAsync("Get_Attendance", GetSqlParameters(request)).Result.ToList();

            var studentInClasses = this.unitOfWork.Repository<tbl_StudentInClass>().ExcuteStoreAsync("Get_StudentInClassForAttendance", GetSqlParameters(new
            {
                classId = request.classId,
                status = 1
            })).Result.ToList();

            var item = attendances.FirstOrDefault();
            //Lấy danh sách đơn xin nghỉ trong khoảng thời gian này
            var studentLeaves = await this.unitOfWork.Repository<tbl_StudentLeaveRequest>().GetQueryable()
                .Where(x => x.deleted == false
                && x.fromDate <= request.date
                && request.date <= x.toDate)
                .ToListAsync();

            //đoạn này danh cho lần đầu vào màn hình hoặc khi không filter theo status
            if (request.status == 0)
            {
                //nếu không có dữ liệu thì thêm mới dữ liệu điểm danh các học viên trong lớp vào tbl_Attendance
                if (item.totalItem == 0)
                {
                    if (studentInClasses.Any())
                    {
                        attendances = studentInClasses.Select(x => new tbl_Attendance
                        {
                            schoolYearId = _class.schoolYearId,
                            branchId = _class.branchId,
                            classId = _class.id,
                            studentId = x.studentId,
                            studentThumbnail = x.thumbnail,
                            date = request.date,
                            studentFullName = x.fullName,
                            studentCode = x.code
                        }).ToList();
                        item.count = attendances.Count;
                        item.totalItem = attendances.Count;
                        item.available = attendances.Count;
                        //kiểm tra nghỉ phép của các bé có xin nghỉ trong khoảng thời gian này
                        if (studentLeaves.Any())
                        {
                            //lấy những thằng điểm danh có trong danh sách student này 
                            var studentWithRequests = attendances.Where(x => studentLeaves.Select(d => d.studentId).Contains(x.studentId)).ToList();
                            item.available -= studentWithRequests.Count(x => x.status == 1);
                            item.leaveWithoutRequest -= studentWithRequests.Count(x => x.status == 3);
                            item.leaveWithRequest += studentWithRequests.Count;
                            studentWithRequests.ForEach(x => x.status = 2);
                        }
                        // Kiểm tra ngày nghỉ 
                        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds((long)request.date);
                        DateTime date = dateTimeOffset.DateTime.ToLocalTime();
                        int dow = (int)date.DayOfWeek + 1;
                        var dayOfWeek = await this.unitOfWork.Repository<tbl_DayOfWeek>().GetQueryable().
                            FirstOrDefaultAsync(x => x.key == dow && x.deleted == false)
                            ?? throw new AppException(MessageContants.nf_dayOfWeek);
                        var holiday = await this.unitOfWork.Repository<tbl_Holiday>().GetQueryable().
                            AnyAsync(x =>
                            (x.sTime <= request.date && x.eTime >= request.date)
                            && x.deleted == false);
                        if (dayOfWeek.active.Value && !holiday)
                            await this.unitOfWork.Repository<tbl_Attendance>().CreateAsync(attendances);
                    }
                }
                else
                {
                    //nếu có dữ liệu rồi thì xem có khớp với sỉ số lớp không
                    if (studentInClasses.Count > attendances.Count)
                    {
                        //có học viên mới chuyển vào lớp => thêm vào điểm danh
                        var newStudents = studentInClasses.Where(s => !attendances.Any(a => s.studentId == a.studentId));
                        var newAttendances = newStudents.Select(x => new tbl_Attendance
                        {
                            schoolYearId = _class.schoolYearId,
                            branchId = _class.branchId,
                            classId = _class.id,
                            studentId = x.studentId,
                            studentThumbnail = x.thumbnail,
                            date = request.date,
                        }).ToList();
                        await this.unitOfWork.Repository<tbl_Attendance>().CreateAsync(newAttendances);
                        attendances.AddRange(newAttendances);
                    }
                    else if (studentInClasses.Count < attendances.Count)
                    {
                        //có học viên chuyển khỏi lớp -> xóa khỏi danh sách điểm danh
                        var oldStudents = attendances.Where(s => !studentInClasses.Any(a => s.studentId == a.studentId)).Select(x => x.studentId).ToList();
                        var oldAttendances = await this.unitOfWork.Repository<tbl_Attendance>().GetQueryable()
                            .Where(x =>
                            oldStudents.Contains(x.studentId))
                            .ToListAsync();
                        this.unitOfWork.Repository<tbl_Attendance>().Delete(oldAttendances);
                        attendances.RemoveAll(x => oldStudents.Contains(x.studentId));
                    }
                }
            }

            //kiểm tra nghỉ phép của các bé có xin nghỉ trong khoảng thời gian này
            if (studentLeaves.Any())
            {
                //lấy những thằng điểm danh có trong danh sách student này 
                var studentWithRequests = attendances.Where(x => studentLeaves.Select(d => d.studentId).Contains(x.studentId)).ToList();
                studentWithRequests.ForEach(x => x.leaveRequest = studentLeaves.FirstOrDefault(d => d.studentId == x.studentId));
            }

            result.items = item.count > 0 ? attendances : new List<tbl_Attendance>();
            result.totalItem = item.totalItem;
            result.available = item.available;
            result.leaveWithRequest = item.leaveWithRequest;
            result.leaveWithoutRequest = item.leaveWithoutRequest;
            await this.unitOfWork.SaveAsync();
            return result;
        }
        public async Task GenerateAttendance(tbl_Class _class, Guid studentId)
        {
            var getFirstStudent = await this.unitOfWork.Repository<tbl_Attendance>().GetQueryable().FirstOrDefaultAsync(x =>
            x.classId == _class.id
            && x.deleted == false);

            if (getFirstStudent != null)
            {
                var studentInClassExist = await this.unitOfWork.Repository<tbl_Attendance>().GetQueryable().Where(x =>
                x.classId == _class.id
                && x.studentId == studentId
                && x.deleted == false).Distinct().ToListAsync();

                var studentInClass = await this.unitOfWork.Repository<tbl_Attendance>().GetQueryable().Where(x =>
                x.classId == _class.id
                && x.studentId == getFirstStudent.studentId.Value
                && x.deleted == false).Distinct().ToListAsync();
                // Tạo điểm danh
                List<tbl_Attendance> attendances = new List<tbl_Attendance>();
                attendances = studentInClass.Select(x => new tbl_Attendance
                {
                    schoolYearId = _class.schoolYearId,
                    branchId = _class.branchId,
                    classId = _class.id,
                    studentId = studentId,
                    date = x.date
                }).ToList();
                this.unitOfWork.Repository<tbl_Attendance>().Delete(studentInClassExist);
                await this.unitOfWork.Repository<tbl_Attendance>().CreateAsync(attendances);
                await this.unitOfWork.SaveAsync();
            }
        }
        public async Task SendNotification(tbl_Class _class, tbl_Student student, int status)
        {
            List<Guid> studentIds = new List<Guid>();
            studentIds.Add(student.id);
            var statusName = CoreContants.GetAttendanceStatusName(status);
            //get data parent 
            var receivers = await this.parentService.GetParentUserByStudentId(studentIds);

            List<IDictionary<string, string>> notiParamList = new List<IDictionary<string, string>>();
            foreach (var u in receivers)
            {
                IDictionary<string, string> notiParam = new Dictionary<string, string>();
                notiParam.Add("[StudentName]", student.fullName);
                notiParam.Add("[ClassName]", _class.name);
                notiParam.Add("[StatusName]", statusName);
                notiParamList.Add(notiParam);
            }
            sendNotificationService.SendNotification(Guid.Parse("ea66645b-49d8-40dc-06f3-08dc3c550eb5"), receivers, notiParamList, null, null, null, LookupConstant.ScreenCode_Attendance, null);

        }
        public async Task SendNotification(AttendanceNotificationRequest request)
        {
            //validate
            var collectionSession = await this.unitOfWork.Repository<tbl_Class>()
                .Validate(request.classId.Value) ?? throw new AppException(MessageContants.nf_class);

            //get student ids
            var studentIds = await unitOfWork.Repository<tbl_StudentInClass>()
                            .GetQueryable()
                            .Where(x => x.deleted == false && x.classId == collectionSession.id && x.status == 1 && x.studentId.HasValue)
                            .Select(x => x.studentId.Value)
                            .ToListAsync();

            //get data parent 
            var users = await this.parentService.GetParentUserByStudentId(studentIds);

            //send notification
            List<IDictionary<string, string>> notiParams = new List<IDictionary<string, string>>();
            List<IDictionary<string, string>> emailParams = new List<IDictionary<string, string>>();
            Dictionary<string, string> deepLinkQueryDic = new Dictionary<string, string>();
            Dictionary<string, string> param = new Dictionary<string, string>();

            sendNotificationService.SendNotification_v2(LookupConstant.NCC_Attendance,
                request.title,
                request.content,
                users,
                notiParams,
                emailParams,
                null,
                deepLinkQueryDic,
                LookupConstant.ScreenCode_DailyActivities,
                param);
        }
    }
}
