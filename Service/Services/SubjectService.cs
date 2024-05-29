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
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;

namespace Service.Services
{
    public class SubjectService : DomainService<tbl_Subject, SubjectSearch>, ISubjectService
    {
        private readonly IAppDbContext appDbContext; 
        public SubjectService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext appDbContext) : base(unitOfWork, mapper)
        {
            this.appDbContext = appDbContext;
        }
        protected override string GetStoreProcName()
        {
            return "Get_Subject";
        }
        public override async Task Validate(tbl_Subject model)
        {
            if (model.subjectGroupId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_SubjectGroup>().GetQueryable().FirstOrDefaultAsync(x => x.id == model.subjectGroupId) ?? throw new AppException(MessageContants.nf_subjectGroup);
            }
        }

        public async Task<List<SubjectPrepare>> GetSubjectToPrepareTimeTable(SubjectPrepareSearch request)
        {
            List<SubjectPrepare> subjects = new List<SubjectPrepare>();

            string stringParams = GenerateParamsString(request);

            subjects = await this.appDbContext.Set<SubjectPrepare>().FromSqlRaw($"Get_SubjectToPrepare {stringParams}" ).ToListAsync();

            return subjects;
        }
    }
}
