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
using System.Security.Cryptography.Xml;
using API.Model;
using AppDbContext;
using Entities.AuthEntities;
using Service;
using Microsoft.EntityFrameworkCore.Storage;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Azure.Core;
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Quản lý công nợ")]
    [Authorize]
    public class BillController : BaseController<tbl_Bill, BillCreate, BillUpdate, BillSearch>
    {
        private readonly ISchoolYearService schoolYearService;
        private readonly IStudentService studentService;
        private readonly IAutoGenCodeConfigService autoGenCodeConfigService;
        private readonly IBranchService branchService;
        private readonly IBillDetailService billDetailService;
        private readonly IBillService billService;
        private readonly ITuitionConfigService tuitionConfigService;
        private readonly IDiscountService discountService;
        private readonly IPaymentSessionService paymentSessionService;
        private readonly IStudentInClassService studentInClassService;
        private readonly IClassService classService;
        private readonly IGradeService gradeService;
        private readonly IAppDbContext appDbContext;
        public BillController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Bill, BillCreate, BillUpdate, BillSearch>> logger
            , IWebHostEnvironment env
            , IDomainHub hubcontext) : base(serviceProvider, logger, env
            )
        {
            this.schoolYearService = serviceProvider.GetRequiredService<ISchoolYearService>();
            this.studentService = serviceProvider.GetRequiredService<IStudentService>();
            this.autoGenCodeConfigService = serviceProvider.GetRequiredService<IAutoGenCodeConfigService>();
            this.branchService = serviceProvider.GetRequiredService<IBranchService>();
            this.billDetailService = serviceProvider.GetRequiredService<IBillDetailService>();
            this.billService = serviceProvider.GetRequiredService<IBillService>();
            this.tuitionConfigService = serviceProvider.GetRequiredService<ITuitionConfigService>();
            this.discountService = serviceProvider.GetRequiredService<IDiscountService>();
            this.paymentSessionService = serviceProvider.GetRequiredService<IPaymentSessionService>();
            this.studentInClassService = serviceProvider.GetRequiredService<IStudentInClassService>();
            this.classService = serviceProvider.GetRequiredService<IClassService>();
            this.gradeService = serviceProvider.GetRequiredService<IGradeService>();
            this.appDbContext = serviceProvider.GetRequiredService<IAppDbContext>();
        }

        [HttpGet]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public override async Task<AppDomainResult> Get([FromQuery] BillSearch request)
        {
            var data = await this.billService.GetPagedListData(request);
            return new AppDomainResult(data);
        }

        [HttpGet("{id}")]
        [AppAuthorize]
        [Description("Lấy thông tin")]
        public override async Task<AppDomainResult> GetById(Guid id)
        {
            var bill = await this.billService.GetByIdAsync(id);
            if (bill == null)
                throw new AppException("Không tìm thấy đợt thu");
            return new AppDomainResult(bill);
        }

        [HttpGet("class-by-grade")]
        [AppAuthorize]
        [Description("lấy danh sách lớp từ khối")]
        public async Task<AppDomainResult> GetClassByGrade([FromQuery] ClassInBillSearch request)
        {
            var data = await this.billService.GetClassByGrade(request);
            return new AppDomainResult(data);
        }

        [HttpGet("student-by-class")]
        [AppAuthorize]
        [Description("lấy danh sách học viên từ lớp")]
        public async Task<AppDomainResult> GetStudentByClass([FromQuery] FilterInBillSearch request)
        {
            var data = await this.billService.GetStudentByClass(request);
            return new AppDomainResult(data);
        }

        [HttpGet("detail")]
        [AppAuthorize]
        [Description("Chi tiết bill")]
        public async Task<AppDomainResult> GetDetail([FromQuery] BillDetailSearch request)
        {
            var data = await this.billService.GetDetail(request);
            return new AppDomainResult(data);
        }

        [HttpGet("payment-session")]
        [AppAuthorize]
        [Description("Danh sách phiên thanh toán của bill")]
        public async Task<AppDomainResult> GetPaymentSession([FromQuery] PaymentSessionSearch request)
        {
            var data = await this.billService.GetPaymentSession(request);
            return new AppDomainResult(data);
        }

        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] BillCreate itemModel)
        {          
            using (var tran = await appDbContext.Database.BeginTransactionAsync())
            {         
                try
                {
                    if (!ModelState.IsValid)
                        throw new AppException(ModelState.GetErrorMessage());
                    var listBill = new List<tbl_Bill>();
                    var userLogin = LoginContext.Instance.CurrentUser;
                    double totalPrice = 0;
                    double totalReduced = 0;
                    foreach (var detail in itemModel.billDetails)
                    {
                        var tuitionConfig = await tuitionConfigService.GetByIdAsync(detail.tuitionConfigId);
                        if (tuitionConfig == null)
                            throw new AppException("Không tìm thấy khoản thu");
                        
                        //tổng tiền của khoản thu này bằng tiền khoản thu * số lượng
                        var totalPriceItem = (tuitionConfig.price ?? 0) * detail.quantity;
                        totalPrice += totalPriceItem;

                        //nếu giảm tiền mặt thì cộng thẳng vào
                        if(detail.discountId != null)
                        {
                            var discount = await discountService.GetByIdAsync(detail.discountId.Value);
                            if (discount == null)
                                throw new AppException("Không tìm thấy giảm giá");
                            if (discount.type == 1)
                            {
                                totalReduced += ((discount.value ?? 0) * detail.quantity);
                            }
                            //nếu giảm % thì kiểm tra xem số tiền giảm vượt qua max chưa nếu vượt qua thì gán bằng max
                            else
                            {
                                double reduced = (tuitionConfig.price ?? 0) * ((discount.value ?? 0) / 100);
                                if (reduced > discount.maximumValue)
                                {
                                    reduced = (discount.maximumValue ?? 0);
                                }
                                totalReduced += (reduced * detail.quantity);
                            }
                        }
                        
                    }
                    var totalFinalPrice = totalPrice - totalReduced;
                    var listStudent = await studentService.GetAllAsync();
                    var listStudentId = new List<Guid>();
                    listStudentId = itemModel.itemIds;                        
                    foreach (var studentId in listStudentId)
                    {
                        var student = listStudent.FirstOrDefault(x => x.id == studentId);
                        if (student == null)
                            throw new AppException("Không tìm thấy học viên");
                        var item = new tbl_Bill();
                        item.code = await autoGenCodeConfigService.AutoGenCode(nameof(tbl_Bill));
                        item.totalPrice = totalPrice;
                        item.totalReduced = totalReduced;
                        item.totalFinalPrice = totalFinalPrice;
                        item.paid = 0;
                        item.debt = totalFinalPrice;
                        item.studentId = studentId;
                        item.branchId = student.branchId;
                        item.note = itemModel.Note;
                        item.date = itemModel.date;
                        //tạo mới đợt thu mặc định trạng thái chưa thanh toán
                        item.status = 1;
                        item.statusName = tbl_Bill.GetStatusName(1);
                        await this.billService.AddItem(item);
                        listBill.Add(item);

                        //lưu thông tin chi tiết của bill
                        foreach (var detail in itemModel.billDetails)
                        {
                            var tuitionConfig = await tuitionConfigService.GetByIdAsync(detail.tuitionConfigId);
                            if (tuitionConfig == null)
                                throw new AppException("Không tìm thấy khoản thu");

                            var billDetail = new tbl_BillDetail();
                            billDetail.billId = item.id;
                            billDetail.tuitionConfigId = detail.tuitionConfigId;
                            billDetail.price = tuitionConfig.price ?? 0;
                            billDetail.quantity = detail.quantity;
                            billDetail.totalPrice = tuitionConfig.price * detail.quantity;
                            billDetail.totalFinalPrice = billDetail.totalPrice;
                            if (detail.discountId != null)
                            {
                                var discount = await discountService.GetByIdAsync(detail.discountId.Value);
                                if (discount == null)
                                    throw new AppException("Không tìm thấy giảm giá");

                                billDetail.discountId = detail.discountId;
                                billDetail.discountType = discount.type;
                                billDetail.discountValue = discount.value;
                                billDetail.discountMaximumValue = discount.maximumValue;

                                //giảm tiền mặt thì lấy giá trị value
                                if (discount.type == 1)
                                {
                                    billDetail.reduced = discount.value ?? 0;
                                    billDetail.totalReduced = (discount.value ?? 0) * detail.quantity;
                                }
                                //giảm % thì tính số tiền được giảm
                                else
                                {
                                    double reduced = (tuitionConfig.price ?? 0) * ((discount.value ?? 0) / 100);
                                    //nếu tiền giảm vượt quá max thì set = max
                                    if (reduced > discount.maximumValue)
                                    {
                                        reduced = (discount.maximumValue ?? 0);
                                    }
                                    billDetail.reduced = reduced;
                                    billDetail.totalReduced = reduced * detail.quantity;
                                }
                                billDetail.totalFinalPrice = billDetail.totalPrice - billDetail.totalReduced;
                            }

                            await billDetailService.AddItem(billDetail);
                        }
                    }
                    await tran.CommitAsync();
                    return new AppDomainResult(listBill);
                }
                catch (AppException e)
                {
                    await tran.RollbackAsync();
                    throw e;
                }
            }           
        }

        [HttpDelete("{id}")]
        [AppAuthorize]
        [Description("Xóa")]
        public override async Task<AppDomainResult> DeleteItem(Guid id)
        {
            using (var tran = await appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var userLogin = LoginContext.Instance.CurrentUser;
                    var bill = await billService.GetByIdAsync(id);
                    if (bill == null)
                        throw new Exception("Không tìm thấy đợt thu");
                    await this.billService.DeleteItem(id);

                    var listPaymentSession = await paymentSessionService.GetAsync(x => x.deleted == false && x.billId == bill.id);
                    if(listPaymentSession.Count > 0)
                    {
                        foreach (var item in listPaymentSession)
                        {
                            await paymentSessionService.DeleteItem(item.id);
                        }
                    }                                   
                    await tran.CommitAsync();
                    return new AppDomainResult();
                }
                catch (AppException e)
                {
                    await tran.RollbackAsync();
                    throw e;
                }
            }
        }

        [HttpPost("payment")]
        [AppAuthorize]
        [Description("Thanh toán")]
        public async Task<AppDomainResult> Payment([FromBody] PaymentsRequest itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            await billService.Payment(itemModel);
            return new AppDomainResult();
        }

        [HttpPost("mobile-payment")]
        [AppAuthorize]
        [Description("Thanh toán app mobile")]
        public async Task<AppDomainResult> MobilePayment([FromBody] PaymentsRequest itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            await billService.MobilePayment(itemModel);
            return new AppDomainResult();
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost("add-student")]
        [AppAuthorize]
        [Description("Thêm mới học sinh")]
        public async Task<AppDomainResult> AddStudent([FromBody] StudentCreate itemModel)
        {

            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_Student>(itemModel);
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            item.code = await this.autoGenCodeConfigService.AutoGenCode(nameof(tbl_Student));
            await studentService.Validate(item);
            await studentService.AddItem(item);
            return new AppDomainResult(item);
        }

        /*[HttpGet("excel/export-template")]
        [AppAuthorize]
        [Description("Export")]
        public async Task<AppDomainResult> ExportTemplate()
        {
            var fileUrl = "";
            fileUrl = await this.billService.ExportTemplate();
            return new AppDomainResult(fileUrl);
        }
        [HttpPost("excel/prepare-data")]
        [AppAuthorize]
        [Description("Prepare Data")]
        public async Task<AppDomainResult> PrepareData(IFormFile file)
        {

            var list = await this.billService.PrepareData(file);
            return new AppDomainResult(list);

        }
        /// <summary>
        /// Thêm mới nhieu hoc vien
        /// </summary>
        /// <param name="List itemModel"></param>
        /// <returns></returns>
        [HttpPost("add-list-student")]
        [AppAuthorize]
        [Description("Thêm mới nhiều học viên")]
        public async Task<AppDomainResult> AddListItem([FromBody] List<StudentCreate> itemModel)
        {

            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            foreach (var i in itemModel)
            {
                var item = mapper.Map<tbl_Student>(i);
                if (item == null)
                    throw new AppException(MessageContants.nf_item);
                item.code = await this.autoGenCodeConfigService.AutoGenCode(nameof(tbl_Student));
                await studentService.Validate(item);
                await studentService.AddItem(item);
            }
            return new AppDomainResult(itemModel);
        }*/

        ///// <summary>
        ///// Lấy thông tin theo id
        ///// </summary>
        ///// <param name="billId"></param>
        ///// <returns></returns>
        //[HttpGet("detail/{billId}")]
        //[AppAuthorize]
        //[Description("Lấy thông tin")]
        //public async Task<AppDomainResult> GetDetail(Guid billId)
        //{
        //    var item = (await billDetailService.GetAsync(x => x.billId == billId && x.deleted == false)).Select(b => new BillDetailDTO
        //    {
        //        id = b.id,
        //        billId = b.billId,
        //        note = b.note,
        //        price = b.price,
        //        tuitionConfigDetailId = b.tuitionConfigDetailId
        //    }).ToList();
        //    return new AppDomainResult(item);
        //}

    }
}
