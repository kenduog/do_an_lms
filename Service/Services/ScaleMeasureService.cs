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
using Azure.Core;
using Request.RequestCreate;
using Microsoft.Extensions.DependencyInjection;
using Entities.AuthEntities;
using Entities.DataTransferObject;

namespace Service.Services
{
    public class ScaleMeasureService : DomainService<tbl_ScaleMeasure, ScaleMeasureSearch>, IScaleMeasureService
    {
        private readonly IParentService parentService;
        private readonly ISendNotificationService sendNotificationService;
        private readonly IExcelExportService excelExportService;
        public ScaleMeasureService(IAppUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider) : base(unitOfWork, mapper)
        {
            this.parentService = serviceProvider.GetRequiredService<IParentService>();
            this.sendNotificationService = serviceProvider.GetRequiredService<ISendNotificationService>();
            this.excelExportService = serviceProvider.GetRequiredService<IExcelExportService>();
        }
        protected override string GetStoreProcName()
        {
            return "Get_ScaleMeasures";
        }

        public override async Task Validate(tbl_ScaleMeasure model)
        {
            if (model.branchId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Branch>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.branchId)
                    ?? throw new AppException(MessageContants.nf_branch);
            }
            if (model.schoolYearId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_SchoolYear>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.schoolYearId)
                    ?? throw new AppException(MessageContants.nf_schoolYear);
            }
            if (!string.IsNullOrWhiteSpace(model.gradeIds))
            {
                var branchIds = model.gradeIds.Split(',').Distinct();
                var itemCount = await this.unitOfWork.Repository<tbl_Grade>().GetQueryable().CountAsync(x => branchIds.Contains(x.id.ToString()));
                if (branchIds.Count() != itemCount)
                    throw new AppException(MessageContants.nf_grade);
            }
            if (!string.IsNullOrWhiteSpace(model.classIds))
            {
                var branchIds = model.classIds.Split(',').Distinct();
                var itemCount = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().CountAsync(x => branchIds.Contains(x.id.ToString()));
                if (branchIds.Count() != itemCount)
                    throw new AppException(MessageContants.nf_class);
            }
        }

        public override async Task AddItem(tbl_ScaleMeasure model)
        {
            if (model.type == 1)
            {
                model.classIds = model.gradeIds = null;
            }
            else if (model.type == 2) //theo khối
            {
                model.classIds = null;
            }
            else if (model.type == 3)
            {
                model.gradeIds = null;
            }

            await this.Validate(model);
            await this.CreateAsync(model);

            //tạo các record detail
            var studentInClasses = this.unitOfWork.Repository<tbl_StudentInClass>().ExcuteStoreAsync("Get_StudentInClassForScaleMeasure", GetSqlParameters(new
            {
                classIds = model.classIds,
                gradeIds = model.gradeIds,
                status = 1
            })).Result.ToList();

            if (studentInClasses.Any())
            {
                //generate
                var now = Timestamp.Now();
                var scaleMeasureDetails = studentInClasses.Select(x => new tbl_ScaleMeasureDetail
                {
                    classId = x.classId,
                    gradeId = x.gradeId,
                    scaleMeasureId = model.id,
                    studentId = x.studentId,
                    studentBirthDay = x.birthday,
                    monthOfAge = (int?)((now - (x.birthday ?? 0)) / (30.44 * 24 * 60 * 60 * 1000)),
                    weight = 0,
                    height = 0,
                    bmi = 0,
                    weightMustHave = 0,
                }).ToList();
                await this.unitOfWork.Repository<tbl_ScaleMeasureDetail>().CreateAsync(scaleMeasureDetails);

                // Gửi thông báo cho phụ huynh học sinh
                var listStudent = String.Join(",", studentInClasses.Select(x => x.studentId.ToString()));
                var student = await this.unitOfWork.Repository<tbl_Student>().GetQueryable().Where(x =>
                listStudent.Contains(x.id.ToString())
                && x.deleted == false).ToListAsync();

                List<tbl_Users> fatherIds = new List<tbl_Users>();
                List<tbl_Users> motherIds = new List<tbl_Users>();
                List<tbl_Users> guardianIds = new List<tbl_Users>();

                string uFatherIds = String.Join(",", student.Where(x => x.fatherId != null).Select(x => x.fatherId.ToString()));
                string uMotherIds = String.Join(",", student.Where(x => x.fatherId != null).Select(x => x.motherId.ToString()));
                string uGuardianIds = String.Join(",", student.Where(x => x.fatherId != null).Select(x => x.guardianId.ToString()));

                // Get phụ huynh
                fatherIds = await this.unitOfWork.Repository<tbl_Parent>().GetQueryable().Where(x => uFatherIds.Contains(x.id.ToString())
                && x.deleted == false).Select(x => new tbl_Users()
                {
                    id = x.userId.Value,
                }).ToListAsync();

                motherIds = await this.unitOfWork.Repository<tbl_Parent>().GetQueryable().Where(x => uMotherIds.Contains(x.id.ToString())
                && x.deleted == false).Select(x => new tbl_Users()
                {
                    id = x.userId.Value,
                }).ToListAsync();

                guardianIds = await this.unitOfWork.Repository<tbl_Parent>().GetQueryable().Where(x => uGuardianIds.Contains(x.id.ToString())
                && x.deleted == false).Select(x => new tbl_Users()
                {
                    id = x.userId.Value,
                }).ToListAsync();

                List<tbl_Users> receiver = new List<tbl_Users>();
                receiver.AddRange(fatherIds);
                receiver.AddRange(motherIds);
                receiver.AddRange(guardianIds);

                List<IDictionary<string, string>> notiParamList = new List<IDictionary<string, string>>();
                string subLink = "/scale-measure/detail/";
                string linkQuery = "scaleMeasureId=" + model.id.ToString();
                foreach (var u in receiver)
                {
                    var parentId = await this.unitOfWork.Repository<tbl_Parent>().GetQueryable().FirstOrDefaultAsync(x => x.userId == u.id
                    && x.deleted == false) ?? new tbl_Parent();
                    var studentName = student.FirstOrDefault(x => x.fatherId == parentId.id || x.motherId == parentId.id || x.guardianId == parentId.id)?.fullName;
                    IDictionary<string, string> notiParam = new Dictionary<string, string>();
                    notiParam.Add("[StudentName]", studentName);
                    notiParam.Add("[ScaleMeasureName]", model.name);
                    notiParamList.Add(notiParam);
                }
                sendNotificationService.SendNotification(Guid.Parse("97316131-029f-45a8-d5c2-08dc36983bde"), receiver.Distinct().ToList(), notiParamList, null, linkQuery, null, LookupConstant.ScreenCode_ScaleMeasure, subLink);

            }
            await this.unitOfWork.SaveAsync();
        }

        public async Task SendNotification(ScaleMeasureNotificationRequest request)
        {
            //validate
            var scaleMeasure = await this.unitOfWork.Repository<tbl_ScaleMeasure>()
                .Validate(request.scaleMeasureId.Value) ?? throw new AppException(MessageContants.nf_scaleMeasure);

            //get student ids
            var studentIds = await unitOfWork.Repository<tbl_ScaleMeasureDetail>()
                            .GetQueryable()
                            .Where(x => x.deleted == false && x.scaleMeasureId == scaleMeasure.id && x.studentId.HasValue)
                            .Select(x => x.studentId.Value)
                            .ToListAsync();
            //get data parent 
            var users = await this.parentService.GetParentUserByStudentId(studentIds);

            ////send notification
            //List<IDictionary<string, string>> notiParams = new List<IDictionary<string, string>>();
            //List<IDictionary<string, string>> emailParams = new List<IDictionary<string, string>>();
            //Dictionary<string, string> deepLinkQueryDic = new Dictionary<string, string>();
            //Dictionary<string, string> param = new Dictionary<string, string>();

            //sendNotificationService.SendNotification_v2(LookupConstant.NCC_ScaleMeasure,
            //    request.title,
            //    request.content,
            //    users,
            //    notiParams,
            //    emailParams,
            //    null,
            //    deepLinkQueryDic,
            //    LookupConstant.ScreenCode_Health,
            //    param);
            List<IDictionary<string, string>> notiParamList = new List<IDictionary<string, string>>();
            string subLink = "/scale-measure/detail/";
            string linkQuery = "scaleMeasureId=" + scaleMeasure.id.ToString();
            foreach (var u in users)
            {
                var parentId = await this.unitOfWork.Repository<tbl_Parent>().GetQueryable().FirstOrDefaultAsync(x => x.userId == u.id
                    && x.deleted == false) ?? new tbl_Parent();
                var studentName = unitOfWork.Repository<tbl_Student>()
                            .GetQueryable().FirstOrDefault(x => x.fatherId == parentId.id || x.motherId == parentId.id || x.guardianId == parentId.id)?.fullName;
                IDictionary<string, string> notiParam = new Dictionary<string, string>();
                notiParam.Add("[StudentName]", studentName);
                notiParam.Add("[ScaleMeasureName]", scaleMeasure.name);
                notiParamList.Add(notiParam);
            }
            sendNotificationService.SendNotification(Guid.Parse("67a8e9cd-25ca-4d80-b0fb-08dc39b9d468"), users.Distinct().ToList(), notiParamList, null, linkQuery, null, LookupConstant.ScreenCode_ScaleMeasure, subLink);
        }
    }
}
