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
using Request.RequestCreate;
using Request.RequestUpdate;
using BaseAPI.Controllers;
using static Utilities.CoreContants;
using Newtonsoft.Json;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Text;
using System.Globalization;
using Entities.AuthEntities;
using Entities.DomainEntities;
using Interface.DbContext;
using System.Drawing.Printing;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Description("Màn hình quản lý người dùng")]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class UserController : BaseController<tbl_Users, UserCreate, UserUpdate, UserSearch>
    {
        protected IUserService userService;
        private IConfiguration configuration;
        private IRoleService roleService;
        private IAppDbContext appDbContext;
        private IUserGroupService userGroupService;
        private IGroupService groupService;
        public UserController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Users, UserCreate, UserUpdate, UserSearch>> logger
            , IConfiguration configuration
            , IRoleService roleService
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IUserService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.appDbContext = serviceProvider.GetRequiredService<IAppDbContext>();
            this.userGroupService = serviceProvider.GetRequiredService<IUserGroupService>();
            this.groupService = serviceProvider.GetRequiredService<IGroupService>();
            this.configuration = configuration;
            this.roleService = roleService;
        }
        #region Ẩn đi

        /// <summary>
        /// Lấy thông tin theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AppAuthorize]
        [Description("Lấy thông tin")]
        [NonAction]
        public override async Task<AppDomainResult> GetById(Guid id)
        {
            return new AppDomainResult();
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        [NonAction]
        public override async Task<AppDomainResult> AddItem([FromBody] UserCreate itemModel)
        {
            return new AppDomainResult();
        }

        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut]
        [AppAuthorize]
        [Description("Chỉnh sửa")]
        [NonAction]
        public override async Task<AppDomainResult> UpdateItem([FromBody] UserUpdate itemModel)
        {
                return new AppDomainResult();
        }

        /// <summary>
        /// Xóa item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AppAuthorize]
        [Description("Xoá")]
        [NonAction]
        public virtual async Task<AppDomainResult> DeleteItem(Guid id)
        {
            try
            {
                await this.domainService.DeleteItem(id);
                return new AppDomainResult();
            }
            catch (AppException e)
            {
                throw new AppException(e.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        [NonAction]
        public virtual async Task<AppDomainResult> Get([FromQuery] BaseSearch baseSearch)
        {
            return new AppDomainResult();
        }
        [NonAction]
        public override async Task Validate(tbl_Users model)
        {
            //await userService.ValidateUser(model);
            //if (string.IsNullOrEmpty(model.roleCode) || model.roleCode == Group.Admin.ToString())
            //    throw new AppException("Mã quyền không phù hợp");
        }
        #endregion


        /// <summary>
        /// Cập nhật onesignal player id
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("OneSignalId")]
        [AppAuthorize]
        [Description("Cập nhật OneSignalPlayerId")]
        public async Task<AppDomainResult> UpdateOneSignalId(OneSignalUpdate itemModel)
        {
            if(itemModel == null)
                throw new AppException("Vui lòng nhập dữ liệu");

            var item = await domainService.GetByIdAsync(itemModel.id);
            if(item == null)
                throw new AppException("Người dùng không tồn tại");

            bool success = await userService.UpdateOnesignalPlayerId(itemModel);
            return new AppDomainResult
            {
                success = success,
                resultCode = success ? (int)HttpStatusCode.OK : (int)HttpStatusCode.NotImplemented
            };
        }

        /// <summary>
        /// Thêm mới tài khoản admin, chỉ dev dùng
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost("admin")]
        [AppAuthorize]
        [Description("Thêm mới tài khoản admin, chỉ dev dùng")]
        public async Task<AppDomainResult> AddAdmin([FromBody] AdminCreate itemModel)
        {
            using (var tran = await appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (!ModelState.IsValid)
                        throw new AppException(ModelState.GetErrorMessage());
                    var item = mapper.Map<tbl_Users>(itemModel);
                    if (item == null)
                        throw new AppException(MessageContants.nf_item);
                    await userService.ValidateUser(item);
                    await Validate(item);
                    await this.domainService.AddItem(item);
                    var adminGroup = await groupService.GetByCode("QTV");
                    await userGroupService.AddItem(new tbl_UserGroup
                    {
                        active = true,
                        created = Timestamp.Now(),
                        deleted = true,
                        groupId = adminGroup.id,
                        updated = Timestamp.Now(),
                        userId = item.id,
                    });
                    await tran.CommitAsync();
                    return new AppDomainResult();
                }
                catch (AppException e)
                {
                    await tran.RollbackAsync();
                    throw new AppException(e.Message);
                }
            }
        }
        [HttpGet("admin")]
        [AppAuthorize]
        [Description("Lấy danh sách quản trị viên")]
        public async Task<AppDomainResult> GetAdmin([FromQuery] BaseSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.userService.GetAdmin(baseSearch);

            int totalPage = 0;
            decimal count = data.Item1;
            if (count > 0)
                totalPage = (int)Math.Ceiling(count / baseSearch.pageSize);
            var pagedData = new
            {
                items = data.Item2,
                pageIndex = baseSearch.pageIndex,
                pageSize = baseSearch.pageSize,
                totalItem = data.Item1,
                totalPage = totalPage
            };
            return new AppDomainResult(pagedData);
        }
        /// <summary>
        /// Cập nhật thông tin quản trị viên
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("admin")]
        [AppAuthorize]
        [Description("Chỉnh sửa thông tin quản trị viên")]
        public async Task<AppDomainResult> UpdateAdmin([FromBody] AdminUpdate itemModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new AppException(ModelState.GetErrorMessage());
                var userId = LoginContext.Instance.CurrentUser.userId;
                if (itemModel.id != userId)
                    throw new AppException("Không thể cập nhật thông tin của quản trị viên khác");
                var item = mapper.Map<tbl_Users>(itemModel);
                await userService.ValidateUser(item);
                await Validate(item);
                if (item == null)
                    throw new KeyNotFoundException(MessageContants.nf_item);

                await this.domainService.UpdateItem(item);

                return new AppDomainResult();
            }
            catch (AppException e)
            {
                throw new AppException(e.Message);
            }
        }

        ///// <summary>
        ///// Đọc file excel, chỉ cho phép truyền file excel
        ///// </summary>
        ///// <param name="file"></param>
        ///// <returns></returns>
        //[HttpPost("read-excel")]
        //[AppAuthorize]
        //[Description("Đọc file excel")]
        //public async Task<AppDomainResult> ReadExcel(IFormFile file)
        //{
        //    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //    var result = new List<UserImport>();
        //    IExcelDataReader reader = null;
        //    Stream FileStream = file.OpenReadStream();
        //    DataSet dsexcelRecords = new DataSet();
        //    if (!file.FileName.EndsWith(".xls") && !file.FileName.EndsWith(".xlsx"))
        //        throw new AppException("Tệp tin không đúng định dạng");

        //    reader = ExcelReaderFactory.CreateReader(FileStream);

        //    dsexcelRecords = reader.AsDataSet();
        //    reader.Close();
        //    FileStream.Close();

        //    if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
        //    {
        //        int startCol = 1;
        //        int col = startCol;
        //        DataTable dtStudentRecords = dsexcelRecords.Tables[0];
        //        for (int i = 2; i < dtStudentRecords.Rows.Count; i++)
        //        {
        //            var item = new UserImport();
        //            item.code = dtStudentRecords.Rows[i][col++].ToString();
        //            item.username = dtStudentRecords.Rows[i][col++].ToString();
        //            item.password = dtStudentRecords.Rows[i][col++].ToString();
        //            //item.password = SecurityUtilities.HashSHA1(dtStudentRecords.Rows[i][col++].ToString());
        //            item.roleCode = dtStudentRecords.Rows[i][col++].ToString();
        //            item.firstName = dtStudentRecords.Rows[i][col++].ToString();
        //            item.lastName = dtStudentRecords.Rows[i][col++].ToString();
        //            item.fullName = dtStudentRecords.Rows[i][col++].ToString();
        //            item.gender = convertToInt(dtStudentRecords.Rows[i][col++].ToString());
        //            item.phone = dtStudentRecords.Rows[i][col++].ToString();
        //            item.email = dtStudentRecords.Rows[i][col++].ToString();
        //            item.birthday = convertToDate(dtStudentRecords.Rows[i][col++].ToString());
        //            item.address = dtStudentRecords.Rows[i][col++].ToString();

        //            result.Add(item);
        //            col = startCol;
        //        }
        //    }
        //    AppDomainResult appDomainResult = new AppDomainResult();
        //    appDomainResult.success = true;
        //    appDomainResult.resultCode = (int)HttpStatusCode.OK;
        //    appDomainResult.resultMessage = "Thành công";
        //    appDomainResult.data = result;
        //    return appDomainResult;
        //}

        ///// <summary>
        ///// Lưu danh sách người dùng
        ///// </summary>
        ///// <param name="itemModel"></param>
        ///// <returns></returns>
        //[HttpPost("import-data")]
        //[AppAuthorize]
        //[Description("Lưu danh sách người dùng")]
        //public async Task<AppDomainResult> ImportData([FromBody] UserImports itemModel)
        //{
        //    AppDomainResult appDomainResult = new AppDomainResult();
        //    bool success = false;
        //    if (!ModelState.IsValid)
        //        throw new AppException(ModelState.GetErrorMessage());

        //    if (!itemModel.items.Any())
        //            throw new AppException("Không tìm thấy dữ liệu");
        //        var items = new List<tbl_Users>();

        //    foreach (var item in itemModel.items)
        //    {
        //        var model = mapper.Map<tbl_Users>(item);
        //        if (model != null)
        //        {
        //            await Validate(model);
        //            var countCode = itemModel.items.Count(x => x.code.ToUpper() == item.code.ToUpper());
        //            if (countCode > 1)
        //                throw new AppException($"Trùng mã tài khoản {item.code}");
        //            var countUsername = itemModel.items.Count(x => x.username.ToUpper() == item.username.ToUpper());
        //            if (countCode > 1)
        //                throw new AppException($"Trùng tên đăng nhập {item.username}");
        //            model.password = SecurityUtilities.HashSHA1(item.password);
        //            items.Add(model);
        //        }
        //        else
        //            throw new AppException("Item không tồn tại");
        //    }
        //    success = await this.domainService.CreateAsync(items);
        //    if (success)
        //        appDomainResult.resultCode = (int)HttpStatusCode.OK;
        //    else
        //        throw new Exception("Lỗi trong quá trình xử lý");
        //    appDomainResult.success = success;
        //    return appDomainResult;
        //}


        private double? convertToDate(string dateStr)
        {
            DateTime tempDate;
            string[] formats = {
                "dd/M/yyyy",
                "dd/MM/yyyy",
                "d/M/yyyy",
                "d/MM/yyyy",
                "DD/MM/yyyy",
                "MM/dd/yyyy",
                "MM/dd/yyyy"
            };
            if (DateTime.TryParse(dateStr, out tempDate))
                return Timestamp.TimestampDateTime(tempDate);
            if (DateTime.TryParseExact(dateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
                return Timestamp.TimestampDateTime(tempDate);
            return null;
        }
        private int? convertToInt(string value)
        {
            if (int.TryParse(value, out int newValue))
                return newValue;
            return null;
        }
    }
}
