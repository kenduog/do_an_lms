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
using Entities.AuthEntities;
using Entities.DataTransferObject;
using Microsoft.Extensions.DependencyInjection;

namespace Service.Services
{
    public class StudentInClassService : DomainService<tbl_StudentInClass, StudentInClassSearch>, IStudentInClassService
    {
        private IAppDbContext coreDbContext;
        public ISendNotificationService sendNotificationService;
        public StudentInClassService(IAppUnitOfWork unitOfWork, IServiceProvider serviceProvider, IMapper mapper, IAppDbContext coreDbContext) : base(unitOfWork, mapper)
        {
            this.coreDbContext = coreDbContext;
            this.sendNotificationService = serviceProvider.GetRequiredService<ISendNotificationService>();
        }
        protected override string GetStoreProcName()
        {
            return "Get_StudentInClass";
        }
        public async Task<List<UserOption>> GetStudentAvailable(StudentAvailableSearch baseSearch)
        {
            var item = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().FirstOrDefaultAsync(x => x.id == baseSearch.classId) ?? throw new AppException(MessageContants.nf_class);
            var data = await this.unitOfWork.Repository<tbl_Student>().GetDataExport("Get_StudentAvailable_StudentInClass", new SqlParameter[]
            {
                new SqlParameter("classId", item.id),
                new SqlParameter("schoolYearId", item.schoolYearId),
                new SqlParameter("branchId", item.branchId),
            });
            var result = new List<UserOption>();
            if (data.Any())
            {
                result = data.Select(x => new UserOption
                {
                    id = x.id,
                    code = x.code,
                    name = x.fullName
                }).Distinct().ToList();
            }
            return result;
        }
        public async Task<Guid> GetClassIdByStudent(Guid studentId)
        {
            var studentInClass = await this.unitOfWork.Repository<tbl_StudentInClass>().GetQueryable().FirstOrDefaultAsync(x => x.studentId == studentId && x.deleted == false && x.status == (int)CoreContants.StudentInClassStatus.dang_hoc);
            if (studentInClass == null)
                return Guid.Empty;
            return studentInClass.classId ?? Guid.Empty;
        }
        public async Task<string> GetClassNameByStudent(Guid studentId)
        {
            var studentInClass = await this.unitOfWork.Repository<tbl_StudentInClass>().GetQueryable().FirstOrDefaultAsync(x => x.studentId == studentId && x.deleted == false && x.status == (int)CoreContants.StudentInClassStatus.dang_hoc);
            if (studentInClass == null)
                return "";
            var item = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().FirstOrDefaultAsync(x => x.id == studentInClass.classId && x.deleted == false);
            if (item == null)
                return "";
            return item.name;
        }
        public async Task<List<tbl_StudentInClass>> GetClassOfStudent(Guid studentId, List<tbl_StudentInClass> allClasses, tbl_Student student)
        {

            if(allClasses == null)
            {
                throw new AppException("Không tìm thấy");
            }

            foreach(var i in allClasses)
            {
                i.birthday = student.birthday;
                i.fullName = student.fullName;
                i.code = student.code;
                i.thumbnail = student.thumbnail;
                i.gender = student.gender;
                i.genderName = student.genderName;
                i.type = student.type;
                i.typeName = student.typeName;
                i.nickname = student.nickname;
                i.className =await GetClassNameByStudent(studentId);
                
            }

            return allClasses;
        }




        public async Task SendNotification(tbl_Student student, tbl_Class classes)
        {
            // Gửi thông báo cho phụ huynh học sinh
            List<tbl_Users> fatherIds = new List<tbl_Users>();
            List<tbl_Users> motherIds = new List<tbl_Users>();
            List<tbl_Users> guardianIds = new List<tbl_Users>();

            // Get phụ huynh
            fatherIds = await this.unitOfWork.Repository<tbl_Parent>().GetQueryable().Where(x => x.id == (student.fatherId ?? Guid.Empty)
                    && x.deleted == false).Select(x => new tbl_Users()
                    {
                        id = x.userId.Value,
                    }).ToListAsync();

            motherIds = await this.unitOfWork.Repository<tbl_Parent>().GetQueryable().Where(x => x.id == (student.motherId ?? Guid.Empty)
                    && x.deleted == false).Select(x => new tbl_Users()
                    {
                        id = x.userId.Value,
                    }).ToListAsync();

            guardianIds = await this.unitOfWork.Repository<tbl_Parent>().GetQueryable().Where(x => x.id == (student.guardianId ?? Guid.Empty)
                    && x.deleted == false).Select(x => new tbl_Users()
                    {
                        id = x.userId.Value,
                    }).ToListAsync();

            List<tbl_Users> receiver = new List<tbl_Users>();
            receiver.AddRange(fatherIds);
            receiver.AddRange(motherIds);
            receiver.AddRange(guardianIds);
            if (!receiver.Any())
                return;
            List<IDictionary<string, string>> notiParamList = new List<IDictionary<string, string>>();
            string subLink = "/preschool-class/class/detail/";
            string linkQuery = "classId=" + classes.id.ToString();
            foreach (var u in receiver)
            {
                IDictionary<string, string> notiParam = new Dictionary<string, string>();
                notiParam.Add("[StudentName]", student.fullName);
                notiParam.Add("[ClassName]", classes.name);
                notiParamList.Add(notiParam);
            }
            sendNotificationService.SendNotification(Guid.Parse("c8104488-9713-464a-d5c3-08dc36983bde"), receiver.Distinct().ToList(), notiParamList, null, linkQuery, null, LookupConstant.ScreenCode_Class, subLink);

        }
    }
}
