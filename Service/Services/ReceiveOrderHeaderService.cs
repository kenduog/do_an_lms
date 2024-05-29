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

namespace Service.Services
{
    public class ReceiveOrderHeaderService : DomainService<tbl_ReceiveOrderHeader, ReceiveOrderHeaderSearch>, IReceiveOrderHeaderService
    {
        private readonly IAutoGenCodeConfigService autoGenCode;
        public ReceiveOrderHeaderService(IAppUnitOfWork unitOfWork, IMapper mapper, IAutoGenCodeConfigService autoGenCode) : base(unitOfWork, mapper)
        {
            this.autoGenCode = autoGenCode;
        }


        protected override string GetStoreProcName()
        {
            return "Get_ReceiveOrderHeader";
        }

        public override async Task Validate(tbl_ReceiveOrderHeader model)
        {
            if (model.branchId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Branch>().Validate(model.branchId.Value) ?? throw new AppException(MessageContants.nf_branch);
            }
            if (model.vendorId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Vendor>().Validate(model.vendorId.Value) ?? throw new AppException(MessageContants.nf_vendor);
            }
            if (model.details != null && model.details.Count > 0)
            {
                var itemCount = await this.unitOfWork.Repository<tbl_ItemOfSKU>().GetQueryable()
                    .CountAsync(x => model.details.Select(d => d.itemSkuId).Contains(x.id) && x.deleted == false);
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
                var validateCode = await this.unitOfWork.Repository<tbl_ReceiveOrderHeader>().GetQueryable()
                .AnyAsync(x => x.deleted == false && x.id != model.id && x.code == model.code);
                if (validateCode)
                    throw new AppException(MessageContants.exs_code);
            }
        }

        public override async Task AddItem(tbl_ReceiveOrderHeader model)
        {
            await this.Validate(model);

            //validate code, init auto-gen-code if null or empty
            if (string.IsNullOrEmpty(model.code))
            {
                var code = await autoGenCode.AutoGenCode(nameof(tbl_ReceiveOrderHeader));
                model.code = code;
            }

            List<tbl_ReceiveOrderLine> details = model.details.ToList();
            //re-calc values (totalProduct, totalItem, amt) from FE 
            model.totalProduct = details.Count;
            //model.totalItem = details.Sum(x => x.qty);
            model.amt = details.Sum(x => x.unitPrice * x.qty);

            //add item-header
            await this.unitOfWork.Repository<tbl_ReceiveOrderHeader>().CreateAsync(model);

            //add item-details
            details.ForEach(x =>
            {
                x.amt = x.unitPrice * x.qty;
                x.receiveOrderId = model.id;
            });
            await this.unitOfWork.Repository<tbl_ReceiveOrderLine>().CreateAsync(details);
            await this.unitOfWork.SaveAsync();
        }

        public override async Task UpdateItem(tbl_ReceiveOrderHeader model)
        {
            await this.Validate(model);

            //validate code, init auto-gen-code if null or empty
            if (string.IsNullOrEmpty(model.code))
            {
                var code = autoGenCode.AutoGenCode(nameof(tbl_ReceiveOrderHeader));
            }

            List<tbl_ReceiveOrderLine> details = model.details.ToList();
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
                x.receiveOrderId = model.id;
            });

            var currentItems = await this.unitOfWork.Repository<tbl_ReceiveOrderLine>().GetQueryable().Where(x => x.deleted == false && x.receiveOrderId == model.id).ToListAsync();
            if (currentItems.Any())
            {
                //get deleted Ids to delete
                var deletedItems = currentItems.Where(x => !details.Select(d => d.id).Contains(x.id)).ToList();

                //new items
                var newItems = details.Where(x => !currentItems.Select(d => d.id).Contains(x.id)).ToList();

                //update items
                var updatedItems = details.Where(x => currentItems.Select(d => d.id).Contains(x.id)).ToList();

                //submit value
                await this.unitOfWork.Repository<tbl_ReceiveOrderLine>().CreateAsync(newItems);
                this.unitOfWork.Repository<tbl_ReceiveOrderLine>().UpdateRange(updatedItems);
                this.unitOfWork.Repository<tbl_ReceiveOrderLine>().Delete(deletedItems);
            }

            await this.unitOfWork.SaveAsync();
        }

        public async Task ChangeStatus(ReceiveOrderHeaderStatusUpdate request)
        {
            var item = await this.unitOfWork.Repository<tbl_ReceiveOrderHeader>().GetQueryable().FirstOrDefaultAsync(x => x.id == request.id)
                ?? throw new AppException(MessageContants.nf_receiveOrderHeader);

            if (string.IsNullOrEmpty(request.statusCode) || item.statusCode == request.statusCode)
                throw new AppException(MessageContants.invalid_action);

            //không thể hủy phiếu đã duyệt
            if(item.statusCode == LookupConstant.TrangThai_DaDuyet && request.statusCode == LookupConstant.TrangThai_DaHuy)
                throw new AppException(MessageContants.can_not_cancel_approved_bill);

            item.statusCode = request.statusCode;
            
            switch (request.statusCode)
            {
                case LookupConstant.TrangThai_DaDuyet:
                    item.approvedBy = LoginContext.Instance.CurrentUser?.userId;
                    item.approvedDate = Timestamp.Now();

                    //duyệt phiếu xuất thì cộng những item vào kho 
                    var details1 = await this.unitOfWork.Repository<tbl_ReceiveOrderLine>().GetQueryable().Where(x => x.deleted == false && x.receiveOrderId == item.id).ToListAsync();
                    var inventories1 = await this.unitOfWork.Repository<tbl_ItemInventory>().GetQueryable()
                        .Where(x => x.deleted == false
                        && x.branchId == item.branchId
                        && details1.Select(d => d.itemId).Contains(x.itemId))
                        .ToListAsync();
                    foreach(var detail in details1)
                    {
                        var inventory = inventories1.FirstOrDefault(x => x.itemId == detail.itemId);
                        if (inventory == null)
                        {
                            inventory = new tbl_ItemInventory
                            {
                                itemId = detail.itemId,
                                branchId = item.branchId,
                                qty = detail.qty * detail.convertQty,
                            };
                            inventories1 .Add(inventory);
                            await this.unitOfWork.Repository<tbl_ItemInventory>().CreateAsync(inventory);
                        }
                        else
                        {
                            inventory.qty += detail.qty * detail.convertQty;
                            this.unitOfWork.Repository<tbl_ItemInventory>().Update(inventory);
                        }
                    }

                    //và lưu thẻ kho
                    var valueEntries = details1.Select(x => new tbl_ValueEntry
                    {
                        branchId = item.branchId,
                        itemId = x.itemId,
                        itemSkuId = x.itemSkuId,
                        billId = item.id,
                        vendorId = item.vendorId,
                        price = x.unitPrice,
                        qty = x.qty,
                        type = 1,
                        date = item.date,
                        convertQty = x.convertQty,
                    }).ToList();
                    await this.unitOfWork.Repository<tbl_ValueEntry>().CreateAsync(valueEntries);
                    break;

                case LookupConstant.TrangThai_DangMo:
                    item.isApproved = true;
                    item.approvedBy = null;
                    item.approvedDate = null;
                    item.isApproved = false;

                    //mở lại phiếu nhập thì trừ đi những item đã nhập kho
                    var details2 = await this.unitOfWork.Repository<tbl_ReceiveOrderLine>().GetQueryable().Where(x => x.deleted == false && x.receiveOrderId == item.id).ToListAsync();
                    var inventories2 = await this.unitOfWork.Repository<tbl_ItemInventory>().GetQueryable()
                        .Where(x => x.deleted == false
                        && x.branchId == item.branchId
                        && details2.Select(d => d.itemId).Contains(x.itemId))
                        .ToListAsync();

                    
                    foreach (var detail in details2)
                    {
                        var inventory = inventories2.FirstOrDefault(x => x.itemId == detail.itemId);

                        //nếu số lượng tồn ít hơn số lượng cần xuất thì thông báo 
                        if (inventory == null || inventory.qty < detail.qty)
                        {
                            var currentItem = await this.unitOfWork.Repository<tbl_Item>().GetQueryable().FirstOrDefaultAsync(x => x.id == detail.itemId);
                            string message = $"Tồn kho của `{currentItem.code} - {currentItem.name}` chỉ còn {inventory?.qty ?? 0}, không đủ để mở lại phiếu nhập kho";
                            throw new AppException(message);
                        }
                        //trừ tồn kho
                        inventory.qty -= detail.qty * detail.convertQty;
                    }

                    //Xóa thẻ kho
                    var valueEntries2 = await this.unitOfWork.Repository<tbl_ValueEntry>().GetQueryable().Where(x => x.billId == item.id).ToListAsync();
                    this.unitOfWork.Repository<tbl_ValueEntry>().Delete(valueEntries2);
                    this.unitOfWork.Repository<tbl_ItemInventory>().UpdateRange(inventories2);
                    break;

                case LookupConstant.TrangThai_DaHuy:
                    //chỉ được phép mở lại phiếu đang mở, nên không cần cập nhật tồn kho và thẻ kho
                    item.isApproved = false;
                    break;
            }

            //save change
            this.unitOfWork.Repository<tbl_ReceiveOrderHeader>().Update(item);
            await this.unitOfWork.SaveAsync();
        }
    }
}
