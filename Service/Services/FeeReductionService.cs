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
    public class FeeReductionService : DomainService<tbl_FeeReduction, FeeReductionSearch>, IFeeReductionService
    {
        private readonly IFeeReductionConfigService feeReductionConfigService;
        public FeeReductionService(IAppUnitOfWork unitOfWork, IMapper mapper, IFeeReductionConfigService feeReductionConfigService) : base(unitOfWork, mapper)
        {
            this.feeReductionConfigService = feeReductionConfigService;
        }
        protected override string GetStoreProcName()
        {
            return "Get_FeeReduction";
        }

        public override async Task AddItem(tbl_FeeReduction model)
        {
            //validate
            await this.Validate(model);

            //create model
            await this.unitOfWork.Repository<tbl_FeeReduction>().CreateAsync(model);

            //mapping items
            var items = model.items;
            items.ForEach(x => x.feeReductionId = model.id);

            //create items
            await this.unitOfWork.Repository<tbl_FeeReductionConfig>().CreateAsync(items);

            //save change
            await this.unitOfWork.SaveAsync();
        }

        public override async Task UpdateItem(tbl_FeeReduction model)
        {
            //validate 
            await this.Validate(model);

            //update model
            await this.UpdateAsync(model);

            //update items
            var details = model.items;
            var currentItems = await this.unitOfWork.Repository<tbl_FeeReductionConfig>().GetQueryable().Where(x => x.deleted == false && x.feeReductionId == model.id).ToListAsync();
            if (currentItems.Any())
            {
                //get deleted Ids to delete
                var deletedItems = currentItems.Where(x => !details.Select(d => d.id).Contains(x.id)).ToList();
                deletedItems.ForEach(x=>x.deleted = true);

                //new items
                var newItems = details.Where(x => !currentItems.Select(d => d.id).Contains(x.id)).ToList();

                //update items
                var updatedItems = details.Where(x => currentItems.Select(d => d.id).Contains(x.id)).ToList();

                //submit value
                await this.unitOfWork.Repository<tbl_FeeReductionConfig>().CreateAsync(newItems);
                await this.feeReductionConfigService.UpdateAsync(updatedItems);
                await this.feeReductionConfigService.UpdateAsync(deletedItems);
            }

            //save change 
            await this.unitOfWork.SaveAsync();
        }

        public override async Task<tbl_FeeReduction> GetByIdAsync(Guid id)
        {
            var item = await this.unitOfWork.Repository<tbl_FeeReduction>().Validate(id) ?? throw new AppException(MessageContants.nf_feeReduction);

            //mapping items
            item.items = await this.unitOfWork.Repository<tbl_FeeReductionConfig>().GetDataExport("Get_All_FeeReductionConfig", new SqlParameter[]
            {
                new SqlParameter("feeReductionId", item.id)
            });

            return item;
        }
    }
}
