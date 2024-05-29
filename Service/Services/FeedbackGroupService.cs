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

namespace Service.Services
{
    public class FeedbackGroupService : DomainService<tbl_FeedbackGroup, FeedbackGroupSearch>, IFeedbackGroupService
    {
        public FeedbackGroupService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext appDbContext) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_FeedbackGroup";
        }

        public override async Task Validate(tbl_FeedbackGroup model)
        {
            if (model.branchId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Branch>().GetQueryable().FirstOrDefaultAsync(x=>x.deleted == false && x.id == model.branchId.Value)
                    ?? throw new AppException(MessageContants.nf_branch);
            }
        }
    }
}
