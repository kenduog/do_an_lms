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

namespace Service.Services
{
    public class CommentService : DomainService<tbl_Comment, CommentSearch>, ICommentService
    {
        private readonly IParentService parentService;
        private readonly ISendNotificationService sendNotificationService;
        public CommentService(IAppUnitOfWork unitOfWork, 
            IMapper mapper,
            IParentService parentService,
            ISendNotificationService sendNotificationService) : base(unitOfWork, mapper)
        {
            this.parentService = parentService;
            this.sendNotificationService = sendNotificationService;
        }
        //protected override string GetStoreProcName()
        //{
        //    return "Get_Comment";
        //}
        public async Task<PagedList<tbl_Comment>> GetOrGenerateComment(CommentSearch request)
        {
            await this.Validate(new tbl_Comment { branchId = request.branchId, classId = request.classId, date = request.date });
            
            List<tbl_Comment> comments = new List<tbl_Comment>();
            List<tbl_Comment> result = new List<tbl_Comment>();
            PagedList<tbl_Comment> pagedList = new PagedList<tbl_Comment>();
            //lấy dữ liệu điểm danh của theo request
            comments = this.unitOfWork.Repository<tbl_Comment>().ExcuteStoreAsync("Get_Comment", GetSqlParameters(request)).Result.ToList();

            var studentInClasses = this.unitOfWork.Repository<tbl_StudentInClass>().ExcuteStoreAsync("Get_StudentInClassForAttendance", GetSqlParameters(new
            {
                classId = request.classId,
                status = 1
            })).Result.ToList();
            // Không có học sinh nào trong lớp
            if (!studentInClasses.Any())
            {
                pagedList.totalItem = 0;
                pagedList.items = null;
                pagedList.pageIndex = request.pageIndex;
                pagedList.pageSize = request.pageSize;
                return pagedList;
            }
            //nếu không có dữ liệu thì thêm mới dữ liệu điểm danh các học viên trong lớp vào tbl_Attendance
            if (!comments.Any())
            {
                if (studentInClasses.Any())
                {
                    comments = studentInClasses.Select(x => new tbl_Comment
                    {
                        branchId = request.branchId,
                        classId = request.classId,
                        studentId = x.studentId,
                        date = request.date,
                        studentName = x.fullName,
                        studentCode = x.code,
                        studentThumbnail = x.thumbnail
                    }).ToList();
                    await this.unitOfWork.Repository<tbl_Comment>().CreateAsync(comments);
                    await this.unitOfWork.SaveAsync();
                    result = comments;
                    result[0].totalItem = comments.Count;
                    pagedList.totalItem = result[0].totalItem;
                    pagedList.items = result.Skip((request.pageIndex - 1) * request.pageSize).Take(request.pageSize).ToList();
                    pagedList.pageIndex = request.pageIndex;
                    pagedList.pageSize = request.pageSize;
                    return pagedList;
                }
            }
            result = comments;
            pagedList.totalItem = result[0].totalItem;
            pagedList.items = result;
            pagedList.pageIndex = request.pageIndex;
            pagedList.pageSize = request.pageSize;
            await this.unitOfWork.SaveAsync();
            return pagedList;
        }

        public async Task SendNotification(CommentNotificationRequest request)
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

            sendNotificationService.SendNotification_v2(LookupConstant.NCC_Comment,
                request.title,
                request.content,
                users,
                notiParams,
                emailParams,
                null,
                deepLinkQueryDic,
                LookupConstant.ScreenCode_ElectronicCommunications,
                param);
        }
    }
}
