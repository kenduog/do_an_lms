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
using Entities.DataTransferObject;

namespace Service.Services
{
    public class ItemService : DomainService<tbl_Item, ItemSearch>, IItemService
    {
        private IAppDbContext coreDbContext;
        private IAutoGenCodeConfigService autoGenCode;
        public ItemService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext coreDbContext, IAutoGenCodeConfigService autoGenCode) : base(unitOfWork, mapper)
        {
            this.coreDbContext = coreDbContext;
            this.autoGenCode = autoGenCode;
        }
        protected override string GetStoreProcName()
        {
            return "Get_Item";
        }

        public override async Task Validate(tbl_Item model)
        {
            if(model.branchId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Branch>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == model.branchId) ?? throw new AppException(MessageContants.nf_branch);
            }
            if (model.unitOfMeasureId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_UnitOfMeasure>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == model.unitOfMeasureId) ?? throw new AppException(MessageContants.nf_unitOfMeasure);
            }
            if (model.itemGroupId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_ItemGroup>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == model.itemGroupId) ?? throw new AppException(MessageContants.nf_itemGroup);
            }

            //validate exsisted item code
            if(!string.IsNullOrWhiteSpace(model.code))
            {
                var item = await this.unitOfWork.Repository<tbl_Item>().GetQueryable()
                    .AnyAsync(x => x.deleted == false && x.id != model.id && x.code == model.code);
                if (item)
                    throw new AppException(MessageContants.exs_itemCode);
            }
        }

        public override async Task AddItem(tbl_Item model)
        {
            await this.Validate(model);
            if (string.IsNullOrWhiteSpace(model.code))
                model.code = await this.autoGenCode.AutoGenCode(nameof(tbl_Item));
            await this.unitOfWork.Repository<tbl_Item>().CreateAsync(model);
            //add default sku
            var itemSku = new tbl_ItemOfSKU
            {
                itemId = model.id,
                name = model.name,
                nameShort = model.nameShort,
                unitOfMearsureId = model.unitOfMeasureId,
                convertQty = 1,
                isMain = true,
                limitInventory = 0,
                created = model.created,
                createdBy = model.createdBy,
            };
            itemSku.code = await this.autoGenCode.AutoGenCode(nameof(tbl_ItemOfSKU));
            await this.unitOfWork.Repository<tbl_ItemOfSKU>().CreateAsync(itemSku);
            await this.unitOfWork.SaveAsync();
        }
    }
}
