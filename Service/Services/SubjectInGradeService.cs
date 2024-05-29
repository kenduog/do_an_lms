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
using System.Net.WebSockets;

namespace Service.Services
{
    public class SubjectInGradeService : DomainService<tbl_SubjectInGrade, SubjectInGradeSearch>, ISubjectInGradeService
    {
        public SubjectInGradeService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_SubjectInGrade";
        }

        public async Task AddRangeAsync(SubjectInGradeCreate request)
        {
            if (request == null || request.items == null)
                throw new AppException(MessageContants.nf_item);

            var items = request.items;

            var schoolYear = await this.unitOfWork.Repository<tbl_SchoolYear>().GetQueryable().FirstOrDefaultAsync(x => x.id == request.schoolYearId)
                ?? throw new AppException(MessageContants.nf_schoolYear);

            var branch = await this.unitOfWork.Repository<tbl_Branch>().GetQueryable().FirstOrDefaultAsync(x => x.id == request.branchId)
                ?? throw new AppException(MessageContants.nf_branch);


            //validate grade
            var grade = await this.unitOfWork.Repository<tbl_Grade>().GetQueryable().FirstOrDefaultAsync(x => x.id == request.gradeId)
                ?? throw new AppException(MessageContants.nf_grade);


            //validate subject
            var subjectIds = await this.unitOfWork.Repository<tbl_Subject>().GetQueryable().CountAsync(x => items.Select(d => d.subjectId).Contains(x.id));
            if (subjectIds != items.Count)
                throw new AppException(MessageContants.nf_subject);

            //thêm dc rùi nè
            List<tbl_SubjectInGrade> result = new List<tbl_SubjectInGrade>();

            //cập nhật lại những môn học đã có trong lớp này
            var exsistedSubjects = await this.unitOfWork.Repository<tbl_SubjectInGrade>().GetQueryable()
                .Where(x => x.deleted == false && x.gradeId == grade.id && x.schoolYearId == schoolYear.id && x.branchId == branch.id).ToListAsync();

            if (exsistedSubjects.Any())
            {
                var oldItems = items
                        .Where(x => exsistedSubjects.Select(d => d.subjectId).Contains(x.subjectId))
                        .ToDictionary(x => x.subjectId, x => new ItemSubject { lessonInSemester1 = x.lessonInSemester1, lessonInSemester2 = x.lessonInSemester2, duration = x.duration });

                foreach (var exsistedSubject in exsistedSubjects)
                {
                    if (!oldItems.TryGetValue(exsistedSubject.subjectId, out ItemSubject sub))
                    {
                        //continue;
                        exsistedSubject.deleted = true;
                    }
                    else
                    {
                        exsistedSubject.lessonInSemester1 = sub.lessonInSemester1;
                        exsistedSubject.lessonInSemester2 = sub.lessonInSemester2;
                        exsistedSubject.duration = sub.duration;
                    }
                }

                items = items.Where(x => !exsistedSubjects.Select(d => d.subjectId).Contains(x.subjectId)).ToList();
                this.unitOfWork.Repository<tbl_SubjectInGrade>().UpdateRange(exsistedSubjects);
            }
            var subjectInGrades = items.Select(x => new tbl_SubjectInGrade
            {
                branchId = request.branchId,
                schoolYearId = request.schoolYearId,
                gradeId = request.gradeId,
                subjectId = x.subjectId,
                lessonInSemester1 = x.lessonInSemester1,
                lessonInSemester2 = x.lessonInSemester2,
                duration = x.duration,
            });

            result.AddRange(subjectInGrades);

            await this.unitOfWork.Repository<tbl_SubjectInGrade>().CreateAsync(result);
            await this.unitOfWork.SaveAsync();
        }

        public async Task<List<tbl_SubjectInGrade>> GetAllSubjectInGrade (SubjectInGradeSearch request)
        {
            List<tbl_SubjectInGrade> result = new List<tbl_SubjectInGrade>();
            result = this.GetPagedListData(request).Result.items.ToList();
            result.ForEach(x => x.isSelected = true);
            var subjects = await this.unitOfWork.Repository<tbl_Subject>().GetQueryable()
                .Where(x=>x.deleted == false).ToListAsync();
            if (subjects.Any())
            {
                var subjectGroups = await this.unitOfWork.Repository<tbl_SubjectGroup>()
                    .GetQueryable().Where(x => x.branchId == request.branchId && x.deleted == false).ToListAsync();
                var notExsistSubjects = subjects.Where(x => !result.Select(d => d.subjectId).Contains(x.id)
                && subjectGroups.Select(x => x.id).ToList().Contains(x.subjectGroupId ?? Guid.Empty)).ToList();
                if (notExsistSubjects.Any())
                {
                    var appendList = notExsistSubjects.Select(x => new tbl_SubjectInGrade
                    {
                        id = Guid.NewGuid(),
                        subjectId = x.id,
                        gradeId = request.gradeId,
                        schoolYearId = request.schoolYearId,
                        branchId = request.branchId,
                        duration = 0,
                        lessonInSemester1 = x.totalSemester1,
                        lessonInSemester2 = x.totalSemester2,
                        subjectGroupName = subjectGroups.FirstOrDefault(d => d.id == x.subjectGroupId)?.name,
                        subjectName = x.name,
                        subjectRemarkType = x.remarkType,
                        subjectType = x.type,
                        isSelected = false
                    });
                    result.AddRange(appendList);
                }
            }
            return result;
        }

        private class ItemSubject
        {
            public int? duration { get; set; }
            public int? lessonInSemester1 { get; set; }
            public int? lessonInSemester2 { get; set; }
        }
    }
}
