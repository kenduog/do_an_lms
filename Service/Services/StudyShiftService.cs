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
using Request.RequestCreate;
using Interface.Services.DomainServices;

namespace Service.Services
{
    public class StudyShiftService : DomainService<tbl_StudyShift, StudyShiftSearch>, IStudyShiftService
    {
        public StudyShiftService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public override async Task<PagedList<tbl_StudyShift>> GetPagedListData(StudyShiftSearch baseSearch)
        {
            PagedList<tbl_StudyShift> pagedList = new PagedList<tbl_StudyShift>();
            var ids = await unitOfWork.Repository<tbl_StudyShift>().GetQueryable()
                .Where(x => x.deleted == false)
                .OrderByDescending(x => x.created)
                .Select(x => x.id).ToListAsync();
            if (ids.Any())
            {
                pagedList.totalItem = ids.Count();
            }

            var data = ids.Skip((baseSearch.pageIndex - 1) * baseSearch.pageSize).Take(baseSearch.pageSize).ToList();
            pagedList.items = (from i in data
                               select Task.Run(() => this.GetByIdAsync(i)).Result).ToList();
            pagedList.pageIndex = baseSearch.pageIndex;
            pagedList.pageSize = baseSearch.pageSize;
            return pagedList;
        }

        public override async Task Validate(tbl_StudyShift model)
        {
            var item = await this.unitOfWork.Repository<tbl_StudyShift>().GetQueryable()
                .FirstOrDefaultAsync(x => x.deleted == false && x.id != model.id
                && x.sTime == model.sTime && x.eTime == model.eTime)
                ?? throw new AppException(MessageContants.exs_studyShift);
        }

        public async Task AddRangeAsync(StudyShiftCreate request)
        {
            //if (request == null || request.items == null || request.items.Count == 0)
            //    throw new AppException(MessageContants.nf_item);

            //var branch = await this.unitOfWork.Repository<tbl_StudyShift>().GetQueryable().FirstOrDefaultAsync(x => x.deleted == false && x.id == request.branchId)
            //    ?? throw new AppException(MessageContants.nf_branch);

            //var items = request.items;

            //var gradeIds = request.gradeIds;
            ////validate grades
            //if (gradeIds != null && gradeIds.Count > 0)
            //{
            //    var countGrade = await this.unitOfWork.Repository<tbl_Grade>().GetQueryable()
            //        .CountAsync(x => gradeIds.Contains(x.id) && x.deleted == false);

            //    if (countGrade != gradeIds.Count)
            //        throw new AppException(MessageContants.nf_grade);
            //}
            //else
            //{
            //    gradeIds = await this.unitOfWork.Repository<tbl_Grade>().GetQueryable().Where(x => x.deleted == false)
            //        .Select(x => x.id).ToListAsync();
            //}

            //var classIds = request.classIds;
            //Dictionary<Guid, Guid> classWithGrade = new Dictionary<Guid, Guid>();
            ////validate class
            //if (classIds != null && classIds.Count > 0)
            //{
            //    var countClass = await this.unitOfWork.Repository<tbl_Class>().GetQueryable()
            //        .Where(x => x.gradeId.HasValue && classIds.Contains(x.id) && x.deleted == false).ToListAsync();

            //    if (countClass.Count != classIds.Count)
            //        throw new AppException(MessageContants.nf_class);

            //    classWithGrade = countClass.ToDictionary(x => x.id, x => x.gradeId.Value);
            //}
            //else
            //{
            //    classWithGrade = await this.unitOfWork.Repository<tbl_Class>().GetQueryable()
            //        .Where(x => x.gradeId.HasValue && gradeIds.Contains(x.gradeId.Value) && x.deleted == false)
            //        .ToDictionaryAsync(x => x.id, x => x.gradeId.Value);
            //}

            ////thêm dc rùi nè
            //List<tbl_SubjectInGrade> result = new List<tbl_SubjectInGrade>();
            //foreach (var classId in classIds)
            //{
            //    //cập nhật lại những môn học đã có trong lớp này
            //    var exsistedSubjects = await this.unitOfWork.Repository<tbl_SubjectInGrade>().GetQueryable()
            //        .Where(x => x.deleted == false && x.classId == classId).ToListAsync();
            //    if (exsistedSubjects.Any())
            //    {
            //        var oldItems = items
            //            .Where(x => exsistedSubjects.Select(d => d.subjectId).Contains(x.subjectId))
            //            .ToDictionary(x => x.subjectId, x => new ItemSubject { lessonInSemester1 = x.lessonInSemester1, lessonInSemester2 = x.lessonInSemester2 });

            //        foreach (var exsistedSubject in exsistedSubjects)
            //        {
            //            if (!oldItems.TryGetValue(exsistedSubject.subjectId, out ItemSubject sub))
            //            {
            //                continue;
            //            }
            //            exsistedSubject.lessonInSemester1 = sub.lessonInSemester1;
            //            exsistedSubject.lessonInSemester2 = sub.lessonInSemester2;
            //        }

            //        items = items.Where(x => !exsistedSubjects.Select(d => d.subjectId).Contains(x.subjectId)).ToList();
            //        this.unitOfWork.Repository<tbl_SubjectInGrade>().UpdateRange(exsistedSubjects);
            //    }
            //    var subjectInGrades = items.Select(x => new tbl_SubjectInGrade
            //    {
            //        gradeId = request.gradeId,
            //        classId = classId,
            //        subjectId = x.subjectId,
            //        lessonInSemester1 = x.lessonInSemester1,
            //        lessonInSemester2 = x.lessonInSemester2,
            //    });
            //    result.AddRange(subjectInGrades);
            //}

            //await this.unitOfWork.Repository<tbl_SubjectInGrade>().CreateAsync(result);
            //await this.unitOfWork.SaveAsync();
        }

        private class ItemSubject
        {
            public int? lessonInSemester1 { get; set; }
            public int? lessonInSemester2 { get; set; }
        }
    }
}
