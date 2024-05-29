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
using Entities.DataTransferObject;

namespace Service.Services
{
    public class ValueEntryService : DomainService<tbl_ValueEntry, ValueEntrySearch>, IValueEntryService
    {
        private readonly IExcelExportService excelExportService;
        public ValueEntryService(IAppUnitOfWork unitOfWork, IMapper mapper, IExcelExportService excelExportService) : base(unitOfWork, mapper)
        {
            this.excelExportService = excelExportService;
        }

        protected override string GetStoreProcName()
        {
            return "Get_ValueEntry";
        }

        private async Task<ValueEntryReport> GetData(ValueEntrySearch baseSearch)
        {
            var result = new ValueEntryReport();

            //validate item
            if (!baseSearch.sTime.HasValue || !baseSearch.eTime.HasValue || baseSearch.sTime >= baseSearch.eTime)
                throw new AppException(MessageContants.cp_date);

            var branch = await this.unitOfWork.Repository<tbl_Branch>().Validate(baseSearch.branchId ?? Guid.Empty) ?? throw new AppException(MessageContants.nf_branch);
            var item = await this.unitOfWork.Repository<tbl_Item>().Validate(baseSearch.itemId ?? Guid.Empty) ?? throw new AppException(MessageContants.nf_item);
            var vendor = await this.unitOfWork.Repository<tbl_Vendor>().Validate(baseSearch.vendorId ?? Guid.Empty);

            //tính đầu kỳ
            var queryDauKy = this.unitOfWork.Repository<tbl_ValueEntry>().GetQueryable().Where(x => x.itemId == item.id && branch.id == x.branchId && x.date < (baseSearch.sTime ?? 0));
            if (vendor != null)
            {
                queryDauKy = queryDauKy.Where(x => x.vendorId == vendor.id);
            }
            result.beginning = queryDauKy.Sum(x => (x.qty ?? 0) * (x.convertQty ?? 0));

            //tính trong kỳ
            var queryTrongKy = this.unitOfWork.Repository<tbl_ValueEntry>().GetQueryable().Where(x => x.itemId == item.id && branch.id == x.branchId
                                && baseSearch.sTime <= x.date && x.date <= baseSearch.eTime);
            if (vendor != null)
            {
                queryTrongKy = queryTrongKy.Where(x => x.vendorId == vendor.id);
            }
            result.duringImport = queryTrongKy.Where(x => x.type == 1).Sum(x => (x.qty ?? 0) * (x.convertQty ?? 0));
            result.duringExport = queryTrongKy.Where(x => x.type == 2).Sum(x => (x.qty ?? 0) * (x.convertQty ?? 0));

            //tính cuối kỳ
            result.ending = result.beginning + result.duringImport - result.duringExport;

            return result;
        }
        public async Task<ValueEntryReport> GetValueEntry(ValueEntrySearch baseSearch)
        {
            var result = await GetData(baseSearch);
            //lấy danh sách
            result.data = await this.GetPagedListData(baseSearch);
            return result;
        }

        public async Task<string> Export (ValueEntrySearch baseSearch)
        {
            var result = await GetData(baseSearch);
            string url = "";
            string templateName = ExcelConstant.Export_ValueEntry;
            string folder = ExcelConstant.Export;
            
            var pars = GetSqlParameters(baseSearch);
            var data = await this.unitOfWork.Repository<tbl_ValueEntry>().GetDataExport("Get_ValueEntry_Export", pars);
            var dataToExportModels = mapper.Map<List<ValueEntryExport>>(data);
            ExcelPayload<ValueEntryExport> payload = new ExcelPayload<ValueEntryExport>()
            {
                items = dataToExportModels,
                templateName = templateName,
                folderToSave = folder,
                fromRow = 4,
            };
            payload.keyValues = new Dictionary<ExcelIndex, string>
            {
                { new ExcelIndex(1,1), $"DANH SÁCH THẺ KHO ({Timestamp.ToString(baseSearch.sTime, "dd/MM/yyyy")} - {Timestamp.ToString(baseSearch.eTime, "dd/MM/yyyy")})"},
                { new ExcelIndex(2,6), $"Đầu kỳ: {result.beginning}"},
                { new ExcelIndex(2,7), $"Nhập trong kỳ: {result.duringImport}"},
                { new ExcelIndex(2,8), $"Xuất trong kỳ: {result.duringExport}"},
                { new ExcelIndex(2,9), $"Cuối kỳ: {result.ending}"},
            };

            url = excelExportService.Export(payload);
            return url;
        }
    }
}
