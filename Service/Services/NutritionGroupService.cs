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

namespace Service.Services
{
    public class NutritionGroupService : DomainService<tbl_NutritionGroup, NutritionGroupSearch>, INutritionGroupService
    {
        public NutritionGroupService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_NutritionGroup";
        }

        public override async Task Validate(tbl_NutritionGroup model)
        {
            if (model.branchId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Branch>().Validate(model.branchId.Value) ?? throw new AppException(MessageContants.nf_branch);
            }
            if (!string.IsNullOrEmpty(model.gradeIds))
            {
                var grades = model.gradeIds.Split(",").ToList();
                var gradeCount = await this.unitOfWork.Repository<tbl_Grade>().GetQueryable().CountAsync(x => x.deleted == false && grades.Contains(x.id.ToString()));
                if (gradeCount != grades.Count)
                    throw new AppException(MessageContants.nf_grade);
            }
        }
    }
}
