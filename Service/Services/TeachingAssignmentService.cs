using AutoMapper;
using Entities;
using Entities.AuthEntities;
using Entities.Search;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Request.RequestCreate;
using Service.Services.DomainServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utilities;

namespace Service.Services
{
    public class TeachingAssignmentService : DomainService<tbl_TeachingAssignment, TeachingAssignmentSearch>, ITeachingAssignmentService
    {
        private readonly IAppDbContext appDbContext;
        public TeachingAssignmentService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext appDbContext) : base(unitOfWork, mapper)
        {
            this.appDbContext = appDbContext;
        }
        protected override string GetStoreProcName()
        {
            return "Get_TeachingAssignment";
        }

        public async Task<List<TeacherBySubjectReponse>> GetTeacherAssignmentBySubject(TeacherBySubjectRequest request)
        {
            List<TeacherBySubjectReponse> subjects = new List<TeacherBySubjectReponse>();

            string stringParams = GenerateParamsString(request);

            subjects = await this.appDbContext.Set<TeacherBySubjectReponse>().FromSqlRaw($"Get_TeachingAssignmentBySubject {stringParams}").ToListAsync();

            return subjects;
        }

        public async Task AddOrUpdate(TeachingAssignmentCreate request)
        {
            //validate 
            var branch = await this.unitOfWork.Repository<tbl_Branch>().GetQueryable()
                .FirstOrDefaultAsync(x => x.deleted == false && x.id == request.branchId) ?? throw new AppException(MessageContants.nf_branch);
            var schoolYear = await this.unitOfWork.Repository<tbl_SchoolYear>().GetQueryable()
                .FirstOrDefaultAsync(x => x.deleted == false && x.id == request.schoolYearId) ?? throw new AppException(MessageContants.nf_schoolYear);
            var subject = await this.unitOfWork.Repository<tbl_Subject>().GetQueryable()
                .FirstOrDefaultAsync(x => x.deleted == false && x.id == request.subjectId) ?? throw new AppException(MessageContants.nf_subject);

            var teachers = await this.unitOfWork.Repository<tbl_Staff>().GetQueryable().Where(x => x.deleted == false && request.teacherIds.Contains(x.id)).ToListAsync();

            if (teachers.Count != request.teacherIds.Count)
                teachers = new List<tbl_Staff>();

            foreach (var item in teachers)
            {
                var teacherInfo = await unitOfWork.Repository<tbl_Users>()
                    .GetQueryable().FirstOrDefaultAsync(x => x.id == item.userId);
                if (teacherInfo == null)
                    throw new AppException(MessageContants.nf_teacher);
                if(!teacherInfo.branchIds.ToLower().Contains(request.branchId.ToString().ToLower()))
                    throw new AppException($"Giáo viên {teacherInfo.fullName} không thuộc chi nhánh {branch.name}");
            }

            //loop for teacher and check if it exists in current teaching assignment, then skip it. Else remove it
            var teachingAssignments = await this.unitOfWork.Repository<tbl_TeachingAssignment>().GetQueryable()
                .Where(x => x.deleted == false && x.subjectId == request.subjectId && x.branchId == branch.id && x.schoolYearId == schoolYear.id)
                .ToListAsync();
            //remove non-exists teacher
            var removeItems = teachingAssignments.Where(x => !request.teacherIds.Contains(x.teacherId)).ToList();
            if (removeItems != null && removeItems.Count > 0)
            {
                removeItems.ForEach(x => x.deleted = true);
                this.unitOfWork.Repository<tbl_TeachingAssignment>().UpdateRange(removeItems);
            }

            var addTeachers = teachers.Where(x => !teachingAssignments.Select(x => x.teacherId).Contains(x.id)).ToList();
            if (addTeachers != null && addTeachers.Count > 0)
            {
                var addItems = addTeachers.Select(x => new tbl_TeachingAssignment
                {
                    branchId = branch.id,
                    schoolYearId = schoolYear.id,
                    teacherId = x.id,
                    subjectId = subject.id,
                }).ToList();
                await this.unitOfWork.Repository<tbl_TeachingAssignment>().CreateAsync(addItems);
            }
            await this.unitOfWork.SaveAsync();
        }
    }
}
