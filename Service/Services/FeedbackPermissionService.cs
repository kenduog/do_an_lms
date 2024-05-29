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
    public class FeedbackPermissionService : DomainService<tbl_FeedbackPermission, FeedbackPermissionSearch>, IFeedbackPermissionService
    {
        public FeedbackPermissionService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "Get_FeedbackPermission";
        }

        public override async Task Validate(tbl_FeedbackPermission model)
        {
            if (!string.IsNullOrEmpty(model.groupIds))
            {
                var groupIds = model.groupIds.Split('\u002C');
                var groupExsist = await this.unitOfWork.Repository<tbl_FeedbackGroup>().GetQueryable()
                    .CountAsync(x => groupIds.Contains(x.id.ToString()));
                if (groupExsist != groupIds.Count())
                    throw new AppException(MessageContants.nf_feedbackGroup);
            }
            if (model.branchId.HasValue)
            {
                var branch = await this.unitOfWork.Repository<tbl_Branch>().GetQueryable()
                    .AnyAsync(x=>x.deleted == false && x.id == model.branchId);
                if (!branch)
                    throw new AppException(MessageContants.nf_branch);
            }
            if (!string.IsNullOrEmpty(model.code)) //validate code is exsisted in current feedback permission
            {
                var isExisted = await this.AnyAsync(x => x.id != model.id && x.code == model.code && x.deleted == false && x.branchId == model.branchId);
                if (isExisted)
                    throw new AppException(MessageContants.exs_groupCode);
            }
        }

        public override async Task<PagedList<tbl_FeedbackPermission>> GetPagedListData(FeedbackPermissionSearch baseSearch)
        {
            var pagedData =await  base.GetPagedListData(baseSearch);
            if (pagedData != null)
            {
                var groups = await this.unitOfWork.Repository<tbl_FeedbackGroup>().GetQueryable()
                    .Where(x => x.deleted == false).ToListAsync();
                foreach (var item in pagedData.items)
                {
                    if (!string.IsNullOrEmpty(item.groupIds))
                    {
                        var groupNames = groups.Where(x => item.groupIds.ToUpper().Contains(x.id.ToString().ToUpper()));
                        item.groupNames_vi = groupNames.Select(x => x.name_vi).ToList();
                    }
                }
            }
            return pagedData;
        }
    }
}
