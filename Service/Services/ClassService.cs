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
using Entities.AuthEntities;
using Microsoft.Extensions.DependencyInjection;
using Request.RequestUpdate;
using Newtonsoft.Json.Converters;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace Service.Services
{
    public class ClassService : DomainService<tbl_Class, ClassSearch>, IClassService
    {
        private readonly ITimeTableService timeTableService;
        private readonly IAppDbContext appDbContext;
        private readonly IClassInTimeTableService classInTimeTableService;
        private readonly IAutoGenCodeConfigService autoGenCodeConfigService;
        private readonly ISendNotificationService sendNotificationService;
        public ClassService(IAppUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeTableService timeTableService,
            IClassInTimeTableService classInTimeTableService,
            ISendNotificationService sendNotificationService,
            IAutoGenCodeConfigService autoGenCodeConfigService,
            IAppDbContext appDbContext) : base(unitOfWork, mapper)
        {
            this.appDbContext = appDbContext;
            this.autoGenCodeConfigService = autoGenCodeConfigService;
            this.timeTableService = timeTableService;
            this.classInTimeTableService = classInTimeTableService;
            this.sendNotificationService = sendNotificationService;
        }
        protected override string GetStoreProcName()
        {
            return "Get_Class";
        }

        public override async Task Validate(tbl_Class model)
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
            if (model.gradeId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Grade>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == model.gradeId)
                    ?? throw new AppException(MessageContants.nf_grade);
            }
            //if (!string.IsNullOrEmpty(model.name))
            //{
            //    var item = await this.unitOfWork.Repository<tbl_Class>().GetQueryable()
            //        .AnyAsync(x => x.id != model.id && x.name == model.name);
            //    if (item)
            //        throw new AppException(MessageContants.exs_name);
            //}
            if (model.teacherId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Staff>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == model.teacherId)
                    ?? throw new AppException(MessageContants.nf_teacher);
                // check teacher used manage class
                var asTeacher = await this.unitOfWork.Repository<tbl_Class>().GetQueryable().AnyAsync(x =>
                x.teacherId == model.teacherId.Value
                && x.id != model.id
                && x.schoolYearId == model.schoolYearId
                && x.deleted == false);
                if (asTeacher)
                    throw new AppException(MessageContants.exs_classTeacher);
            }

        }

        public async Task<List<tbl_Class>> GenerateClass(MultipleClassCreate request)
        {
            var schoolYear = await this.unitOfWork.Repository<tbl_SchoolYear>().GetQueryable()
                                .FirstOrDefaultAsync(x => x.id == request.schoolYearId)
                                ?? throw new AppException(MessageContants.nf_schoolYear);

            ////Tạo 1 bản nháp thời khóa biểu
            //tbl_TimeTable timeTable = new tbl_TimeTable();
            //timeTable.draff = true;
            //timeTable.name = $"Bản nháp {await autoGenCodeConfigService.AutoGenCode(nameof(tbl_TimeTable))}";
            //timeTable.active = false;

            //await this.timeTableService.CreateAsync(timeTable);

            //tạo các lớp được truyền vào và gắn nó vào bản nháp
            List<tbl_Class> classesResult = new List<tbl_Class>();
            if (request.classes != null && request.classes.Count > 0)
            {
                var classes = request.classes;
                foreach (var _class in classes)
                {
                    var item = await this.unitOfWork.Repository<tbl_Grade>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == _class.gradeId)
                    ?? throw new AppException(MessageContants.nf_grade);

                    if (!_class.totalClass.HasValue || _class.totalClass.Value == 0)
                        continue;
                    int classIndex = _class.increaseWith == 1 ? CoreContants.GetAlphabetIndex(_class.firstLetter.ToCharArray()[0]) + 1 : Convert.ToInt16(_class.firstLetter);
                    for (int i = 0; i < _class.totalClass; i++)
                    {
                        string name = "";
                        if (_class.increaseWith == 1)
                        {
                            name = $"{_class.startWith} {CoreContants.GetAlpabetLetter(classIndex + i - 1)}";
                        }
                        else
                        {
                            name = $"{_class.startWith} {classIndex + i}";
                        }
                        tbl_Class tmp = new tbl_Class
                        {
                            gradeId = _class.gradeId,
                            schoolYearId = request.schoolYearId,
                            name = name,
                            size = _class.size,
                        };
                        await this.CreateAsync(tmp);
                        //await this.classInTimeTableService.CreateAsync(new tbl_ClassInTimeTable
                        //{
                        //    classId = tmp.id,
                        //    timeTableId = timeTable.id
                        //});
                        classesResult.Add(tmp);
                    }
                }
            }
            await this.unitOfWork.SaveAsync();

            //return danh sách lớp vừa tạo
            return classesResult;
        }

        public async Task<List<ClassToPrepare>> GetClassToPrepare(ClassPrepare request)
        {
            List<ClassToPrepare> result = new List<ClassToPrepare>();
            string stringParams = GenerateParamsString(request);
            result = await this.appDbContext.Set<ClassToPrepare>().FromSqlRaw($"Get_ClassToPrepare {stringParams}").ToListAsync();
            return result;
        }
        public override async Task<tbl_Class> UpdateItemWithResponse(tbl_Class model)
        {
            await UpdateItem(model);
            // Gửi thông báo nhận lớp cho giáo viên
            var teacher = await this.unitOfWork.Repository<tbl_Staff>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.id && x.deleted == false);
            if (teacher != null)
            {
                var uTeacher = await this.unitOfWork.Repository<tbl_Users>().GetQueryable().FirstOrDefaultAsync(x => x.id == teacher.userId && x.deleted == false);
                if (uTeacher != null)
                {
                    var receiver = new List<tbl_Users>();
                    receiver.Add(uTeacher);
                    List<IDictionary<string, string>> notiParamList = new List<IDictionary<string, string>>();
                    IDictionary<string, string> notiParam = new Dictionary<string, string>();
                    notiParam.Add("[ClassName]", model.name);
                    notiParamList.Add(notiParam);
                    string linkQuery = "classId=" + sendNotificationService.EncodingParam(model.id.ToString());
                    string subLink = string.Empty;
                    sendNotificationService.SendNotification(Guid.Parse("ded2f0e3-ba3e-4969-6fe3-08dc17fc7766"), receiver, notiParamList, null, linkQuery, null, LookupConstant.ScreenCode_TeachingAssignment, subLink);
                }
            }
            return model;
        }
        public async Task<tbl_Class> AddClass(ClassCreate request)
        {
            var model = mapper.Map<tbl_Class>(request);
            await this.Validate(model);
            await this.unitOfWork.Repository<tbl_Class>().CreateAsync(model);

            if (request.shifts != null && request.shifts.Count > 0)
            {
                //qui định giờ học theo ngày
                List<tbl_ClassShift> classShifts = request.shifts.Select(x => new tbl_ClassShift
                {
                    classId = model.id,
                    day = x.day,
                    period = x.period,
                    sTime = x.sTime,
                    eTime = x.eTime
                }).ToList();
                await this.unitOfWork.Repository<tbl_ClassShift>().CreateAsync(classShifts);
            }

            await this.unitOfWork.SaveAsync();
            return model;
        }
        public async Task<tbl_Class> EditClass(ClassUpdate request)
        {
            var entity = await unitOfWork.Repository<tbl_Class>().GetQueryable()
                .FirstOrDefaultAsync(x => x.id == request.id);
            if (entity == null)
                throw new AppException(MessageContants.nf_others);

            entity.name = request.name ?? entity.name;
            entity.size = request.size ?? entity.size;
            entity.teacherId = request.teacherId ?? entity.teacherId;
            entity.schoolYearId = request.schoolYearId ?? entity.schoolYearId;
            await this.Validate(entity);
            this.unitOfWork.Repository<tbl_Class>().Update(entity);

            var shift = await unitOfWork.Repository<tbl_ClassShift>().GetQueryable()
                .Where(x => x.classId == request.id && x.deleted == false).ToListAsync();
            if (request.shifts.Any())
            {
                // tạo ca mới
                if (request.shifts.Any(x => x.id == Guid.Empty || x.id == null))
                {
                    List<tbl_ClassShift> classShifts = request.shifts.Where(x => x.id == Guid.Empty || x.id == null).Select(x => new tbl_ClassShift
                    {
                        classId = entity.id,
                        day = x.day,
                        period = x.period,
                        sTime = x.sTime,
                        eTime = x.eTime
                    }).ToList();
                    await this.unitOfWork.Repository<tbl_ClassShift>().CreateAsync(classShifts);
                }
                foreach (var item in shift)
                {
                    // nếu có thì chỉnh sửa 
                    if (request.shifts.Any(x => x.id == item.id))
                    {
                        var updateData = request.shifts.FirstOrDefault(x => x.id == item.id);
                        item.sTime = updateData.sTime;
                        item.eTime = updateData.eTime;
                    }
                    else
                    {
                        item.deleted = true;
                    }
                }
            }
            // xóa hết ca học
            else if (shift.Count > 0)
            {
                shift.ForEach(x => x.deleted = true);
            }
            this.unitOfWork.Repository<tbl_ClassShift>().UpdateRange(shift);
            await this.unitOfWork.SaveAsync();
            return entity;
        }

        public override async Task<tbl_Class> GetByIdAsync(Guid id)
        {
            var item = await this.unitOfWork.Repository<tbl_Class>().GetSingleRecordAsync("Get_ClassById", new SqlParameter[] { new SqlParameter("id", id) });
            if (item != null)
                item.classShifts = Task.Run(() => ClassShiftByClassAsync(id)).Result;
            return item;
        }

        public async Task<tbl_Class> ClassByIdAsync(Guid id)
        {
            var item = await this.unitOfWork.Repository<tbl_Class>().Validate(id) ?? throw new AppException(MessageContants.nf_class);
            item.classShifts = Task.Run(() => ClassShiftByClassAsync(id)).Result;
            return item;
        }
        public async Task<List<ClassShift>> ClassShiftByClassAsync(Guid classId)
        {
            var classShift = await this.unitOfWork.Repository<tbl_ClassShift>().GetQueryable().Where(x => x.classId == classId && x.deleted == false).ToListAsync();
            var item = new List<ClassShift>();
            foreach (var c in classShift.Select(x => x.day).Distinct())
            {
                var shift = new ClassShift();
                shift.day = c;
                // lấy tiết học
                shift.period = new List<ClassShiftReriod>();
                shift.period.AddRange(classShift.Where(x => x.day == c).OrderBy(x=>x.period).Select(x => new ClassShiftReriod()
                {
                    id = x.id,
                    period = x.period,
                    sTime = x.sTime,
                    eTime = x.eTime,
                }).ToList());
                item.Add(shift);
            }
            return item;
        }
        public async Task<PagedList<tbl_Class>> GetClassEmpty(ClassSearch baseSearch)
        {
            PagedList<tbl_Class> pagedList = new PagedList<tbl_Class>();
            SqlParameter[] parameters = GetSqlParameters(baseSearch);
            pagedList = await this.unitOfWork.Repository<tbl_Class>().ExcuteQueryPagingAsync("Get_Class_Empty", parameters);
            pagedList.pageIndex = baseSearch.pageIndex;
            pagedList.pageSize = baseSearch.pageSize;
            return pagedList;
        }
    }
}
