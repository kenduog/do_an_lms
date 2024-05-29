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
using Interface.Services.DomainServices;
using Request.RequestUpdate;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Service.Services
{
    public class PurchaseOrderHeaderService : DomainService<tbl_PurchaseOrderHeader, PurchaseOrderHeaderSearch>, IPurchaseOrderHeaderService
    {
        private readonly IAutoGenCodeConfigService autoGenCode;
        private readonly IPurchaseOrderLineService purchaseOrderLineService;
        public PurchaseOrderHeaderService(IAppUnitOfWork unitOfWork, IMapper mapper, IAutoGenCodeConfigService autoGenCode, IPurchaseOrderLineService purchaseOrderLineService) : base(unitOfWork, mapper)
        {
            this.autoGenCode = autoGenCode;
            this.purchaseOrderLineService = purchaseOrderLineService;
        }


        protected override string GetStoreProcName()
        {
            return "Get_PurchaseOrderHeader";
        }

        public override async Task Validate(tbl_PurchaseOrderHeader model)
        {
            if (model.branchId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Branch>().Validate(model.branchId.Value) ?? throw new AppException(MessageContants.nf_branch);
            }
            if (model.details != null && model.details.Count > 0)
            {
                var itemCount = await this.unitOfWork.Repository<tbl_Item>().GetQueryable()
                    .CountAsync(x => model.details.Select(d => d.itemId).Contains(x.id) && x.deleted == false);
                if (itemCount != model.details.Count)
                    throw new AppException(MessageContants.nf_item);
            }
            else
            {
                throw new AppException(MessageContants.req_details);
            }

            //validate code
            if (!string.IsNullOrEmpty(model.code))
            {
                var validateCode = await this.unitOfWork.Repository<tbl_PurchaseOrderHeader>().GetQueryable()
                .AnyAsync(x => x.deleted == false && x.id != model.id && x.code == model.code);
                if (validateCode)
                    throw new AppException(MessageContants.exs_code);
            }
        }

        public override async Task AddItem(tbl_PurchaseOrderHeader model)
        {
            await this.Validate(model);

            //validate code, init auto-gen-code if null or empty
            if (string.IsNullOrEmpty(model.code))
            {
                var code = await autoGenCode.AutoGenCode(nameof(tbl_PurchaseOrderHeader));
                model.code = code;
            }

            List<tbl_PurchaseOrderLine> details = model.details.ToList();
            //re-calc values (totalProduct, totalItem, amt) from FE 
            model.totalProduct = details.Count;
            //model.totalItem = details.Sum(x => x.qty);
            model.amt = details.Sum(x => x.unitPrice * x.qty);

            //add item-header
            await this.unitOfWork.Repository<tbl_PurchaseOrderHeader>().CreateAsync(model);

            //add item-details
            details.ForEach(x =>
            {
                x.amt = x.unitPrice * x.qty;
                x.purchaseOrderId = model.id;
            });

            await this.unitOfWork.Repository<tbl_PurchaseOrderLine>().CreateAsync(details);
            await this.unitOfWork.SaveAsync();
        }

        public override async Task UpdateItem(tbl_PurchaseOrderHeader model)
        {
            await this.Validate(model);

            //validate code, init auto-gen-code if null or empty
            if (model.id== Guid.Empty &&  string.IsNullOrEmpty(model.code))
            {
                var code = autoGenCode.AutoGenCode(nameof(tbl_PurchaseOrderHeader));
            }

            List<tbl_PurchaseOrderLine> details = model.details.ToList();
            //re-calc values (totalProduct, totalItem, amt) from FE 
            model.totalProduct = details.Count;
            //model.totalItem = details.Sum(x => x.qty);
            model.amt = details.Sum(x => x.unitPrice);

            //update item header
            await this.UpdateAsync(model);

            //update item-details
            details.ForEach(x =>
            {
                x.amt = x.unitPrice * x.qty;
                x.purchaseOrderId = model.id;
            });

            var currentItems = await this.unitOfWork.Repository<tbl_PurchaseOrderLine>().GetQueryable().Where(x => x.deleted == false && x.purchaseOrderId == model.id).ToListAsync();
            //get deleted Ids to delete
            var deletedItems = currentItems.Where(x => !details.Select(d => d.id).Contains(x.id)).ToList();
            deletedItems.ForEach(x => x.deleted = true);

            //new items
            var newItems = details.Where(x => !currentItems.Select(d => d.id).Contains(x.id)).ToList();

            //update items
            var updatedItems = details.Where(x => currentItems.Select(d => d.id).Contains(x.id)).ToList();

            //submit value
            await this.unitOfWork.Repository<tbl_PurchaseOrderLine>().CreateAsync(newItems);
            await this.purchaseOrderLineService.UpdateAsync(updatedItems);
            await this.purchaseOrderLineService.UpdateAsync(deletedItems);

            await this.unitOfWork.SaveAsync();
        }

        public override async Task<tbl_PurchaseOrderHeader> GetByIdAsync(Guid id)
        {
            var item = await this.unitOfWork.Repository<tbl_PurchaseOrderHeader>().Validate(id) ?? throw new AppException(MessageContants.nf_purchaseOrderHeader);

            item.details = await this.unitOfWork.Repository<tbl_PurchaseOrderLine>().GetDataExport("Get_All_PurchaseOrderLine", new SqlParameter[]
            {
                new SqlParameter("purchaseOrderId", item.id)
            });

            return item;
        }
        public virtual async Task<tbl_PurchaseOrderHeader> ChangeStatus(ChangeStatusPurchase item)
        {
            var purchaseOrderHeader = await this.unitOfWork.Repository<tbl_PurchaseOrderHeader>().Validate(item.id) ?? throw new AppException(MessageContants.nf_purchaseOrderHeader);
            purchaseOrderHeader.statusCode = item.statusCode;
            await this.UpdateAsync(purchaseOrderHeader);
            return purchaseOrderHeader;
        }

    }
}
