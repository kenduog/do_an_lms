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
using System.Collections.Immutable;
using System.Net.WebSockets;
using Entities.AuthEntities;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace Service.Services
{
    public class StudentInAssessmentService : DomainService<tbl_StudentInAssessment, StudentInAssessmentSearch>, IStudentInAssessmentService
    {
        private readonly ISendNotificationService sendNotificationService;
        public StudentInAssessmentService(IAppUnitOfWork unitOfWork,
            IMapper mapper, IAppDbContext appDbContext, ISendNotificationService sendNotificationService) : base(unitOfWork, mapper)
        {
            this.sendNotificationService = sendNotificationService;
        }
        protected override string GetStoreProcName()
        {
            return "Get_StudentInAssessment";
        }
        public async Task<PagedList<tbl_Student>> GetStudentInClassForAssessment(StudentInClassForAssessmentSearch request)
        {

            List<tbl_Student> students = new List<tbl_Student>();
            PagedList<tbl_Student> pagedList = new PagedList<tbl_Student>();
            //lấy dữ liệu điểm danh của theo request
            students = this.unitOfWork.Repository<tbl_Student>().ExcuteStoreAsync("Get_StudentInClassForAssessment", GetSqlParameters(request)).Result.ToList();
            if (!students.Any())
            {
                pagedList.totalItem = 0;
                pagedList.items = null;
                pagedList.pageIndex = request.pageIndex;
                pagedList.pageSize = request.pageSize;
                return pagedList;
            }
            pagedList.totalItem = students[0].totalItem;
            pagedList.items = students;
            pagedList.pageIndex = request.pageIndex;
            pagedList.pageSize = request.pageSize;
            return pagedList;
        }

        public async Task<List<AssessmentItem>> AllCritialByStudent(CriteriaResult request)
        {
            List<AssessmentItem> result = new List<AssessmentItem>();

            //validate
            var student = await this.unitOfWork.Repository<tbl_Student>().GetQueryable().FirstOrDefaultAsync(x => x.id == request.studentId)
                ?? throw new AppException(MessageContants.nf_student);
            var semester = await this.unitOfWork.Repository<tbl_Semester>().GetQueryable().FirstOrDefaultAsync(x => x.deleted == false && x.id == request.semesterId)
                ?? throw new AppException(MessageContants.nf_semester);
            var _class = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().FirstOrDefaultAsync(x => x.id == request.classId)
                ?? throw new AppException(MessageContants.nf_class);

            //lấy danh sách tiêu chí đầy đủ
            var prs = new SqlParameter[] {
                new SqlParameter("branchId", _class.branchId),
                new SqlParameter("gradeId", _class.gradeId),
            };

            var details = await this.unitOfWork.Repository<tbl_ChildAssessmentDetail>().GetDataExport("Get_AllChildAssessmentDetail", prs);

            //map những tiêu chí đã đạt của bé vào biến selected
            var studentInAssessments = await this.unitOfWork.Repository<tbl_StudentInAssessment>().GetQueryable()
                .Where(x => x.assessmentDetailId.HasValue && x.deleted == false && x.studentId == student.id && x.semesterId == semester.id && x.status == true)
                .Select(x => x.assessmentDetailId.Value)
                .ToListAsync();

            if (studentInAssessments.Any())
            {
                details.ForEach(x => x.selected = studentInAssessments.Contains(x.id));
            }

            //selected ra result thoai (chỗ này ae bô lão bảo là phải truy vấn nhiều lần nè :v đừng làm v nha)
            result = details.GroupBy(x => new { x.topicId, x.topicName })
                .Select(x => new AssessmentItem
                {
                    id = x.Key.topicId ?? Guid.NewGuid(),
                    name = x.Key.topicName,
                    childs = x.Select(d => new AssessmentDetailItem
                    {
                        id = d.id,
                        name = d.name,
                        selected = d.selected
                    }).ToList()
                }).ToList();

            return result;
        }
        public async Task SendNotify(tbl_ChildAssessmentTopic childAssessment, tbl_Student student, List<tbl_Users> receivers)
        {
            List<IDictionary<string, string>> notiParamList = new List<IDictionary<string, string>>();
            foreach (var u in receivers)
            {
                IDictionary<string, string> notiParam = new Dictionary<string, string>();
                notiParam.Add("[StudentName]", student.fullName);
                notiParam.Add("[ChildAssessmentName]", childAssessment.name);
                notiParamList.Add(notiParam);
            }
            sendNotificationService.SendNotification(Guid.Parse("f9f8bc23-707e-49f4-8e6f-08dc375ee9e8"), receivers, notiParamList, null, null, null, LookupConstant.ScreenCode_ChildAssessment, null);
        }

    }
}
