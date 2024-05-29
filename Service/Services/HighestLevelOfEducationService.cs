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
    public class HighestLevelOfEducationService : DomainService<tbl_HighestLevelOfEducation, BaseSearch>, IHighestLevelOfEducationService
    {
        public HighestLevelOfEducationService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        public override async Task<PagedList<tbl_HighestLevelOfEducation>> GetPagedListData(BaseSearch baseSearch)
        {
            PagedList<tbl_HighestLevelOfEducation> pagedList = new PagedList<tbl_HighestLevelOfEducation>();
            var ids = await unitOfWork.Repository<tbl_HighestLevelOfEducation>().GetQueryable()
                .Where(x => x.deleted == false && (string.IsNullOrEmpty(baseSearch.searchContent) || x.name.Contains(baseSearch.searchContent)))
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
    }
}
