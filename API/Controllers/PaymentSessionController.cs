using Entities;
using Extensions;
using Interface.Services;
using Interface.Services.Auth;
using Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Models;
using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Request;
using Entities.Search;
using Request.DomainRequests;
using AutoMapper;
using Request.RequestCreate;
using Request.RequestUpdate;
using System.Reflection;
using Newtonsoft.Json;
using Entities.DomainEntities;
using BaseAPI.Controllers;
using Service.Services;
using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;
using System.Reflection.Metadata;
using Interface.DbContext;
using System.Data.Entity;
using System.Drawing.Printing;
using DocumentFormat.OpenXml.Spreadsheet;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Màn hình thu chi")]
    [Authorize]
    public class PaymentSessionController : BaseController<tbl_PaymentSession, PaymentSessionCreate, PaymentSessionUpdate, PaymentSessionSearch>
    {
        private readonly IBranchService branchService;
        private readonly IBillService billService;
        private readonly IAutoGenCodeConfigService autoGenCodeConfigService;
        private readonly IPaymentSessionService paymentSessionService;
        public PaymentSessionController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_PaymentSession, PaymentSessionCreate, PaymentSessionUpdate, PaymentSessionSearch>> logger
            , IWebHostEnvironment env
            , IDomainHub hubcontext) : base(serviceProvider, logger, env
            )
        {
            this.billService = serviceProvider.GetService<IBillService>();
            this.domainService = serviceProvider.GetRequiredService<IPaymentSessionService>();
            this.paymentSessionService = serviceProvider.GetRequiredService<IPaymentSessionService>();
            this.branchService = serviceProvider.GetRequiredService<IBranchService>();
            this.autoGenCodeConfigService = serviceProvider.GetRequiredService<IAutoGenCodeConfigService>();
        }
        [NonAction]
        public override async Task Validate(tbl_PaymentSession model)
        {
            if (model.branchId.HasValue)
            {
                var hasBranch = await branchService.AnyAsync(x => x.id == model.branchId && x.deleted == false);
                if (!hasBranch)
                    throw new AppException("Không tìm thấy chi nhánh");
            }
        }
        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] PaymentSessionCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var userLogin = LoginContext.Instance.CurrentUser;
            var item = mapper.Map<tbl_PaymentSession>(itemModel);
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            if (item.type == 1 && item.studentId == null)
                throw new Exception("Vui lòng chọn học viên cho phiếu thu!");
            item.code = await autoGenCodeConfigService.AutoGenCode(nameof(tbl_PaymentSession));
            //cập nhật lại thông tin in hóa đơn
            item.printContent = Task.Run(() => this.paymentSessionService.GetPrintContent(item, null)).Result;
            await Validate(item);
            await this.domainService.AddItem(item);
            return new AppDomainResult(item);
        }
        /// <summary>
        /// Xóa item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AppAuthorize]
        [Description("Xoá")]
        public override async Task<AppDomainResult> DeleteItem(Guid id)
        {
            try
            {
                var paymentSession = await this.domainService.GetByIdAsync(id);
                if (paymentSession == null)
                    throw new Exception($"Không tìm thấy phiên thanh toán");
                //phiên thanh toán đó đã duyệt thì mới xử lý
                if(paymentSession.billId != null && paymentSession.status == 2)
                {
                    var bill = await billService.GetByIdAsync(paymentSession.billId.Value);
                    if (bill != null)
                    {
                        bill.paid -= paymentSession.value; 
                        bill.debt += paymentSession.value;
                        //tiền nợ > 0 thì cập nhật bill này chưa thanh toán xong
                        if (bill.debt > 0)
                        {
                            bill.status = 3;
                            bill.statusName = tbl_Bill.GetStatusName(3);
                            //nếu tiền nợ = tiền hóa đơn thì chưa thanh toán
                            if (bill.debt == bill.totalFinalPrice)
                            {
                                bill.status = 1;
                                bill.statusName = tbl_Bill.GetStatusName(1);
                            }
                        }                        
                        //tiền nợ <= 0 thì cập nhật bill này đã thanh toán
                        else
                        {
                            bill.status = 4;
                            bill.statusName = tbl_Bill.GetStatusName(4);
                        }
                        var isUpdate = await billService.UpdateAsync(bill);
                        if(isUpdate == false)
                            throw new Exception($"Lỗi cập nhật thông tin đợt thu");
                    }
                }

                await this.domainService.DeleteItem(id);
                return new AppDomainResult();
            }
            catch (AppException e)
            {
                throw new AppException(e.Message);
            }
        }
        [HttpGet]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public override async Task<AppDomainResult> Get([FromQuery] PaymentSessionSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            PagedList<tbl_PaymentSession> pagedData = await this.domainService.GetPagedListData(baseSearch);
            double? totalRevenue = 0;
            double? totalExpense = 0;
            double? totalProfit = 0;
            if (pagedData.items.Any())
            {
                totalRevenue = pagedData.items[0].totalRevenue;
                totalExpense = pagedData.items[0].totalExpense;
                totalProfit = pagedData.items[0].totalProfit;
            }
            return new AppDomainResult
            { 
                resultCode = ((int)HttpStatusCode.OK),
                resultMessage = "Thành công",
                success = true,
                data = new
                {
                    pageIndex = pagedData.pageIndex,
                    pageSize = pagedData.pageSize,
                    totalPage = pagedData.totalPage,
                    totalItem = pagedData.totalItem,
                    items = pagedData.items,
                    totalRevenue = totalRevenue,                   
                    totalExpense = totalExpense,
                    totalProfit = totalProfit,
                }
            };
        }
    }
}
