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
using Request.RequestCreate;

namespace Service.Services
{
    public class TeachingFrameService : DomainService<tbl_TeachingFrame, TeachingFrameSearch>, ITeachingFrameService
    {
        public TeachingFrameService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_TeachingFrame";
        }

        public override async Task Validate(tbl_TeachingFrame model)
        {
            if (model.schoolYearId.HasValue)
            {
                var schoolYear = await this.unitOfWork.Repository<tbl_SchoolYear>().GetQueryable().FirstOrDefaultAsync(x => x.deleted == false && x.id == model.schoolYearId) ?? throw new AppException(MessageContants.nf_schoolYear);
            }
            if (model.gradeId.HasValue)
            {
                var grade = await this.unitOfWork.Repository<tbl_Grade>().GetQueryable().FirstOrDefaultAsync(x => x.deleted == false && x.id == model.gradeId) ?? throw new AppException(MessageContants.nf_grade);
            }
            if (model.subjectId.HasValue)
            {
                var subject = await this.unitOfWork.Repository<tbl_Subject>().GetQueryable().FirstOrDefaultAsync(x => x.deleted == false && x.id == model.subjectId) ?? throw new AppException(MessageContants.nf_subject);
            }
        }

        public async Task<PagedList<TeachingFrameGroupByGrade>> GetTeachingFrameGroupByGrade(TeachingFrameGroupByGradeSearch baseSearch)
        {
            SqlParameter[] sqlParameters = GetSqlParameters(baseSearch);
            string spName = "Get_TeachingFrameGroupByGrade";
            var datas = await this.unitOfWork.Repository<TeachingFrameGroupByGrade>().ExcuteQueryPagingAsync(spName, sqlParameters);
            datas.pageIndex = baseSearch.pageIndex;
            datas.pageSize = baseSearch.pageSize;
            return datas;
        }
    }
}
