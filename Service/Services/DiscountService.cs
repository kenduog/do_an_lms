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
using System.Threading;
using Request.RequestCreate;
using System.Configuration;

namespace Service.Services
{
    public class DiscountService : DomainService<tbl_Discount, BaseSearch>, IDiscountService
    {
        private readonly IAppDbContext appDbContext;
        public DiscountService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext appDbContext) : base(unitOfWork, mapper)
        {
            this.appDbContext = appDbContext;
        }
        public override async Task<tbl_Discount> GetByIdAsync(Guid id)
        {
            var data = await Queryable.Where(e => e.id == id).AsNoTracking().FirstOrDefaultAsync();
            if (data != null)
                data.tuitionConfigName = (await unitOfWork.Repository<tbl_TuitionConfig>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == data.tuitionConfigId))?.name;
            return data;
        }
        public override async Task<PagedList<tbl_Discount>> GetPagedListData(BaseSearch baseSearch)
        {
            PagedList<tbl_Discount> pagedList = new PagedList<tbl_Discount>();
            var ids = await unitOfWork.Repository<tbl_Discount>().GetQueryable()
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
