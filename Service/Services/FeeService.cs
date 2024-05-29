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
    public class FeeService : DomainService<tbl_Fee, FeeSearch>, IFeeService
    {
        private readonly IFeeInGradeService feeInGradeService;
        public FeeService(IAppUnitOfWork unitOfWork, IMapper mapper, IFeeInGradeService feeInGradeService) : base(unitOfWork, mapper)
        {
            this.feeInGradeService = feeInGradeService;
        }
        protected override string GetStoreProcName()
        {
            return "Get_Fee";
        }

        public override async Task Validate(tbl_Fee model)
        {
            if (model.branchId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Branch>().Validate(model.branchId.Value) ?? throw new AppException(MessageContants.nf_branch);
            }

            if(model.items != null && model.items.Count > 0)
            {
                var gradeIds = model.items.Select(x=>x.gradeId).ToList();
                var itemCount = await this.unitOfWork.Repository<tbl_Grade>().GetQueryable().CountAsync(x => x.deleted == false && gradeIds.Contains(x.id));
                if (itemCount != gradeIds.Count)
                    throw new AppException(MessageContants.nf_grade);
            }
        }

        public override async Task AddItem(tbl_Fee model)
        {
            //validate
            await this.Validate(model);

            //create model
            await this.unitOfWork.Repository<tbl_Fee>().CreateAsync(model);

            //mapping items
            var items = model.items;
            items.ForEach(x => x.feeId = model.id);

            //create items
            await this.unitOfWork.Repository<tbl_FeeInGrade>().CreateAsync(items);

            //save change
            await this.unitOfWork.SaveAsync();
        }

        public override async Task UpdateItem(tbl_Fee model)
        {
            //validate 
            await this.Validate(model);

            //update model
            await this.UpdateAsync(model);

            //update items
            var details = model.items;
            var currentItems = await this.unitOfWork.Repository<tbl_FeeInGrade>().GetQueryable().Where(x => x.deleted == false && x.feeId == model.id).ToListAsync();
            
            //get deleted Ids to delete
            var deletedItems = currentItems.Where(x => !details.Select(d => d.id).Contains(x.id)).ToList();
            deletedItems.ForEach(x => x.deleted = true);

            //new items
            var newItems = details.Where(x => !currentItems.Select(d => d.id).Contains(x.id)).ToList();

            //update items
            var updatedItems = details.Where(x => currentItems.Select(d => d.id).Contains(x.id)).ToList();

            //submit value
            await this.unitOfWork.Repository<tbl_FeeInGrade>().CreateAsync(newItems);
            await this.feeInGradeService.UpdateAsync(updatedItems);
            await this.feeInGradeService.UpdateAsync(deletedItems);

            //save change 
            await this.unitOfWork.SaveAsync();
        }

        public override async Task<tbl_Fee> GetByIdAsync(Guid id)
        {
            var item = await this.unitOfWork.Repository<tbl_Fee>().Validate(id) ?? throw new AppException(MessageContants.nf_fee);

            //mapping items
            item.items = await this.unitOfWork.Repository<tbl_FeeInGrade>().GetDataExport("Get_FeeInGrade", new SqlParameter[] { new SqlParameter("feeId", item.id) });

            return item;
        }

        public async Task<PagedList<tbl_Fee>> GetFeeByCollectionPlan(GetFeeByCollectionPlanRequest request)
        {
            var result = new PagedList<tbl_Fee>();

            var grade = await this.unitOfWork.Repository<tbl_CollectionPlan>().GetQueryable()
                               .FirstOrDefaultAsync(x => x.id == request.collectionPlanId && x.deleted == false)
                               ?? throw new AppException(MessageContants.nf_collectionPlan);

            var sqlParameters = GetSqlParameters(request);

            result = await this.unitOfWork.Repository<tbl_Fee>()
                .ExcuteQueryPagingAsync("Get_FeeByCollectionPlan", sqlParameters);
            result.pageSize = request.pageSize;
            result.pageIndex = request.pageIndex;
            return result;
        }
    }
}
