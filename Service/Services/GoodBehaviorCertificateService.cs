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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.CodeAnalysis.Operations;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using System.Drawing.Printing;
using Request.RequestUpdate;
using System.Data.Entity;

namespace Service.Services
{
    public class GoodBehaviorCertificateService : DomainService<tbl_GoodBehaviorCertificate, GoodBehaviorCertificateSearch>, IGoodBehaviorCertificateService
    {
        private readonly IAttendanceService attendanceService;
        private readonly IWeekService weekService;
        private readonly IBranchService branchService;
        private readonly IClassService classService;
        private readonly IParentService parentService;
        private readonly ISendNotificationService sendNotificationService;

        public GoodBehaviorCertificateService(IServiceProvider serviceProvider, IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            this.attendanceService = serviceProvider.GetRequiredService<IAttendanceService>();
            this.weekService = serviceProvider.GetRequiredService<IWeekService>();
            this.branchService = serviceProvider.GetRequiredService<IBranchService>();
            this.classService = serviceProvider.GetRequiredService<IClassService>();
            this.parentService = serviceProvider.GetRequiredService<IParentService>();
            this.sendNotificationService = serviceProvider.GetRequiredService<ISendNotificationService>();
        }
        //protected override string GetStoreProcName()
        //{
        //    return "Get_GoodBehaviorCertificateService";
        //}
        public async Task<PagedList<tbl_GoodBehaviorCertificate>> GetOrGenerateGoodBehaviorCertificate(GoodBehaviorCertificateSearch request)
        {
            var week = this.unitOfWork.Repository<tbl_Week>().GetQueryable().FirstOrDefault(x => x.id == request.weekId)
                ?? throw new AppException(MessageContants.nf_week);
            PagedList<tbl_GoodBehaviorCertificate> pagedList = new PagedList<tbl_GoodBehaviorCertificate>();
            List<tbl_GoodBehaviorCertificate> goodBehaviorCertificates = new List<tbl_GoodBehaviorCertificate>();
            List<tbl_GoodBehaviorCertificate> result = new List<tbl_GoodBehaviorCertificate>();
            //lấy dữ liệu điểm danh của theo request
            goodBehaviorCertificates = this.unitOfWork.Repository<tbl_GoodBehaviorCertificate>().ExcuteStoreAsync("Get_GoodBehaviorCertificate", GetSqlParameters(request)).Result.ToList();
            var studentInClasses = this.unitOfWork.Repository<tbl_StudentInClass>().ExcuteStoreAsync("Get_StudentInClassForAttendance", GetSqlParameters(new
            {
                classId = request.classId,
                status = 1
            })).Result.ToList();
            if (!studentInClasses.Any()){
                pagedList.totalItem = 0;
                pagedList.items = null;
                pagedList.pageIndex = request.pageIndex;
                pagedList.pageSize = request.pageSize;
                return pagedList;
            }
            //nếu không có dữ liệu thì thêm mới dữ liệu điểm danh các học viên trong lớp vào tbl_Attendance
            if (!goodBehaviorCertificates.Any())
            {
                var branch = this.unitOfWork.Repository<tbl_Branch>().GetQueryable()
                    .FirstOrDefault(x => x.id == (request.branchId ?? Guid.Empty)) ?? throw new AppException(MessageContants.nf_branch);

                var classes = this.unitOfWork.Repository<tbl_Class>().GetQueryable()
                 .FirstOrDefault(x => x.id == (request.classId ?? Guid.Empty)) ?? throw new AppException(MessageContants.nf_class);

                if (studentInClasses.Any())
                {
                    goodBehaviorCertificates = studentInClasses.Select(x => new tbl_GoodBehaviorCertificate
                    {
                        branchId = request.branchId,
                        classId = request.classId,
                        studentId = x.studentId,
                        studentName = x.fullName,
                        studentCode = x.code,
                        weekId = request.weekId,
                        branchName = branch.name,
                        className = classes.name,
                        weekName = week.name,
                        status = false
                    }).ToList();
                    await this.unitOfWork.Repository<tbl_GoodBehaviorCertificate>().CreateAsync(goodBehaviorCertificates);
                    await this.unitOfWork.SaveAsync();
                    result = goodBehaviorCertificates;
                    var attendances = this.unitOfWork.Repository<tbl_Attendance>().GetQueryable().Where(x => x.deleted == false).ToList();
                    foreach (var r in result)
                    {
                        r.authorizedAbsence = attendances.Count(x => x.studentId == r.studentId && x.status == 2);
                        r.unauthorizedAbsence = attendances.Count(x => x.studentId == r.studentId && x.status == 3);
                    }
                    result.Select(x => x.totalItem = goodBehaviorCertificates.Count);
                    pagedList.totalItem = goodBehaviorCertificates.Count;
                    pagedList.items = result.Skip((request.pageIndex - 1) * request.pageSize).Take(request.pageSize).ToList();
                    pagedList.pageIndex = request.pageIndex;
                    pagedList.pageSize = request.pageSize;
                    return pagedList;
                }
            }

            result = goodBehaviorCertificates;
            pagedList.totalItem = result[0].totalItem;
            pagedList.items = result;
            pagedList.pageIndex = request.pageIndex;
            pagedList.pageSize = request.pageSize;
            await this.unitOfWork.SaveAsync();
            return pagedList;
        }

        public async Task<IList<tbl_GoodBehaviorCertificate>> GetForNoti(GoodBehaviorCertificateSearch request)
        {
            return await this.unitOfWork.Repository<tbl_GoodBehaviorCertificate>().ExcuteStoreAsync("Get_GoodBehaviorCertificateNoPaging", GetSqlParameters(request));
        } 

        /// <summary>
        /// Lấy thông tin phiếu bé ngoan của bé, theo tuần 
        /// Mỗi ngày trong tuần trả thêm cờ điểm danh hay vắng
        /// </summary>
        /// <returns></returns>
        public async Task<LearningResultByWeek> GetByWeek(WeekReportRequest request)
        {
            LearningResultByWeek result = new LearningResultByWeek();

            //validate
            var _class = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().FirstOrDefaultAsync(x => x.id == request.classId)
                ?? throw new AppException(MessageContants.nf_class);
            var week = await this.unitOfWork.Repository<tbl_Week>().GetQueryable().FirstOrDefaultAsync(x => x.id == request.weekId) 
                ?? throw new AppException(MessageContants.nf_week);
            var student = await this.unitOfWork.Repository<tbl_Student>().GetQueryable().FirstOrDefaultAsync(x => x.id == request.studentId)
                ?? throw new AppException(MessageContants.nf_student);

            //Lấy thông tin phiếu bé ngoan
            result.status = false;
            var goodBehavior = await this.unitOfWork.Repository<tbl_GoodBehaviorCertificate>().GetQueryable()
                .FirstOrDefaultAsync(x => x.deleted == false && x.studentId == student.id && x.classId == request.classId && x.weekId == week.id);
            if(goodBehavior != null)
            {
                result.status = goodBehavior.status;
            }

            //lấy thông tin điểm danh của các ngày đó
            var attendances = await this.unitOfWork.Repository<tbl_Attendance>().GetQueryable()
                .Where(x => x.deleted == false && week.sTime <= x.date && x.date <= week.eTime
                && x.studentId == request.studentId && x.classId == request.classId).ToListAsync();

            //lấy thông tin ngày học. Map ra đúng 7 ngày của tuần
            List<LearningResultItem> details = new List<LearningResultItem>();
            for (int i = 0; i < 7; i++)
            {
                var item = new LearningResultItem
                {
                    id = Guid.NewGuid(),
                    date = week.sTime + i * 24 * 60 * 60 * 1000,
                };
                item.status = attendances.FirstOrDefault(x => Timestamp.ToLocalDateTime(x.date).Day == Timestamp.ToLocalDateTime(item.date).Day)?.status ?? 0;
                details.Add(item);
            }
            result.details = details;
            return result;
        }

        public async Task SendNotification(GoodBehaviorNotificationRequest request)
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

            sendNotificationService.SendNotification_v2(LookupConstant.NCC_GoodBehavior,
                request.title,
                request.content,
                users,
                notiParams,
                emailParams,
                null,
                deepLinkQueryDic,
                LookupConstant.ScreenCode_GoodBehavior,
                param);
        }
    }
}
