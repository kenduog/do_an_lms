using Entities;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Utilities;
using AutoMapper;
using Service.Services.DomainServices;
using Entities.Search;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static Utilities.CoreContants;
using System.Linq.Expressions;
using Entities.DomainEntities;
using Entities.DataTransferObject;

namespace Service.Services
{
    public class ItemInventoryService : DomainService<tbl_ItemInventory, ItemInventorySearch>, IItemInventoryService
    {
        protected readonly IExcelExportService excelExportService;
        public ItemInventoryService(IExcelExportService excelExportService,IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            this.excelExportService = excelExportService;
        }

        protected override string GetStoreProcName()
        {
            return "Get_ItemInventory";
        }

        public async Task<string> Export(ItemInventorySearch baseSearch)
        {
            var branch = await this.unitOfWork.Repository<tbl_Branch>().Validate(baseSearch.branchId ?? Guid.Empty) ?? throw new AppException(MessageContants.nf_branch);
            string url = "";
            string templateName = ExcelConstant.Export_Inventory;
            string folder = ExcelConstant.Export;

            var pars = GetSqlParameters(baseSearch);
            var data = await this.unitOfWork.Repository<tbl_ItemInventory>().GetDataExport("Get_Inventory_Export", pars);
            var dataToExportModels = mapper.Map<List<ItemInventoryExport>>(data);
            ExcelPayload<ItemInventoryExport> payload = new ExcelPayload<ItemInventoryExport>()
            {
                items = dataToExportModels,
                templateName = templateName,
                folderToSave = folder,
                fromRow = 3,
            };
            payload.keyValues = new Dictionary<ExcelIndex, string>
            {
                { new ExcelIndex(1,1), $"TỒN KHO HIỆN HÀNH - CHI NHÁNH {branch.name.ToUpper()}"},
            };

            url = excelExportService.Export(payload);
            return url;
        }


        public async Task<List<InventoryDetailBySKU>> DetailInventory(Guid id)
        {
            List<InventoryDetailBySKU> result = new List<InventoryDetailBySKU>();

            var inventory = await this.unitOfWork.Repository<tbl_ItemInventory>().Validate(id) ?? throw new AppException(MessageContants.nf_item);

            var itemSkus = await this.unitOfWork.Repository<tbl_ItemOfSKU>().GetQueryable()
                .Where(x => x.deleted == false && x.itemId == inventory.itemId)
                .OrderBy(x => x.convertQty)
                .ToListAsync();

            if (itemSkus.Any())
            {
                var unitOfMeasures = await this.unitOfWork.Repository<tbl_UnitOfMeasure>().GetQueryable().ToDictionaryAsync(x => x.id, x => x.name);

                result = itemSkus.Select(x =>
                {
                    var item = new InventoryDetailBySKU
                    {
                        id = x.id,
                        itemName = x.name,
                        itemCode = x.code,
                        convertQty = x.convertQty,
                        qty = Math.Round((double)(inventory.qty / x.convertQty), 2),
                    };
                    if (unitOfMeasures.TryGetValue(x.unitOfMearsureId ?? Guid.Empty, out string name))
                    {
                        item.unitOfMeasureName = name;
                    }
                    return item;
                }).ToList();
            }
            return result;
        }
    }
}
