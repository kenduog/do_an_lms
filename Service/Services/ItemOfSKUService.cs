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

namespace Service.Services
{
    public class ItemOfSKUService : DomainService<tbl_ItemOfSKU, ItemOfSKUSearch>, IItemOfSKUService
    {
        private readonly IAutoGenCodeConfigService autoGenCode;
        public ItemOfSKUService(IAppUnitOfWork unitOfWork, IMapper mapper, IAutoGenCodeConfigService autoGenCode) : base(unitOfWork, mapper)
        {
            this.autoGenCode = autoGenCode;
        }
        protected override string GetStoreProcName()
        {
            return "Get_ItemOfSKU";
        }

        public override async Task Validate(tbl_ItemOfSKU model)
        {
            if (model.itemId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Item>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == model.itemId) ?? throw new AppException(MessageContants.nf_item);
            }
            if (model.unitOfMearsureId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_UnitOfMeasure>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == model.unitOfMearsureId) ?? throw new AppException(MessageContants.nf_unitOfMeasure);
            }

            //validate exsisted item code
            if (!string.IsNullOrWhiteSpace(model.code))
            {
                var item = await this.unitOfWork.Repository<tbl_ItemOfSKU>().GetQueryable()
                    .AnyAsync(x => x.deleted == false && x.id != model.id && x.code == model.code);
                if (item)
                    throw new AppException(MessageContants.exs_itemCode);
            }

            
            //can not update qty and unit of main
            if (model.id != Guid.Empty && model.isMain == true)
            {
                //var item = await this.unitOfWork.Repository<tbl_ItemOfSKU>().GetQueryable()
                //    .FirstOrDefaultAsync(x => x.id != model.id);
                //if (item != null && item.isMain == true && model.isMain == true)
                //{
                //    //can update main false to true
                //    if (model.isMain.HasValue && model.isMain.Value)
                //        throw new AppException(MessageContants.can_not_update_is_main_of_main_sku);
                //    if (model.convertQty.HasValue && model.convertQty != 1)
                //        throw new AppException(MessageContants.can_not_update_qty_of_main_sku);
                //    if (model.unitOfMearsureId.HasValue && model.unitOfMearsureId != item.unitOfMearsureId)
                //        throw new AppException(MessageContants.can_not_update_unit_of_main_sku);
                //}
            }

            //can not create more than 1 main sku
            if(model.id == Guid.Empty)
            {
                var hasMain = await this.unitOfWork.Repository<tbl_ItemOfSKU>().GetQueryable()
                    .AnyAsync(x => x.itemId == model.itemId && x.isMain == true);
                if (hasMain && model.isMain == true)
                    throw new AppException(MessageContants.exs_mainSKU);

                var hasUnit = await this.unitOfWork.Repository<tbl_ItemOfSKU>().GetQueryable()
                    .AnyAsync(x => x.itemId == model.itemId && x.unitOfMearsureId == model.unitOfMearsureId);
                if (!hasMain)
                    throw new AppException(MessageContants.exs_unitSKU);
            }
        }


        public override async Task AddItem(tbl_ItemOfSKU model)
        {
            await this.Validate(model);
            if (string.IsNullOrWhiteSpace(model.code))
                model.code = await this.autoGenCode.AutoGenCode(nameof(tbl_ItemOfSKU));
            await this.unitOfWork.Repository<tbl_ItemOfSKU>().CreateAsync(model);
            await this.unitOfWork.SaveAsync();
        }

        public override async Task UpdateItem(tbl_ItemOfSKU model)
        {
            await this.Validate(model);
            if (model.isMain == true)
                model.convertQty = 1;
            await this.UpdateAsync(model);

            if(model.isMain == true)
            {
                var item = await this.unitOfWork.Repository<tbl_ItemOfSKU>().Validate(model.id) ?? throw new AppException(MessageContants.nf_item);
                //disable any item else
                var data = await this.unitOfWork.Repository<tbl_ItemOfSKU>().GetQueryable()
                    .Where(x=>x.deleted == false && x.itemId == item.itemId && x.id != model.id && x.isMain == true)
                    .ToListAsync();
                data.ForEach(x => x.isMain = false);
                this.unitOfWork.Repository<tbl_ItemOfSKU>().UpdateRange(data);
            }
            await this.unitOfWork.SaveAsync();
        }

        public override async Task DeleteItem(Guid id)
        {
            var item = await this.unitOfWork.Repository<tbl_ItemOfSKU>().Validate(id) ?? throw new AppException(MessageContants.nf_item);
            if (item.isMain == true)
                throw new AppException(MessageContants.can_not_delete_sku_item);

            await this.DeleteAsync(id);
            await this.unitOfWork.SaveAsync();
        }
    }
}
