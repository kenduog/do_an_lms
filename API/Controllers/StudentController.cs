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
using Microsoft.AspNetCore.Http;
using static Utilities.CoreContants;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Identity.Client;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Màn hình học viên")]
    [Authorize]
    public class StudentController : BaseController<tbl_Student, StudentCreate, StudentUpdate, StudentSearch>
    {
        private IStudentService studentService;
        private IAppDbContext appDbContext;
        private IUserService userService;
        private ICommonService commonService;
        private IProfileTemplateService profileTemplateService;
        private IAutoGenCodeConfigService autoGenCodeConfigService;
        private IProfileService profileService;
        private IBranchService branchService;
        private readonly IParentService parentService;
        public StudentController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Student, StudentCreate, StudentUpdate, StudentSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.autoGenCodeConfigService = serviceProvider.GetRequiredService<IAutoGenCodeConfigService>();
            this.profileService = serviceProvider.GetRequiredService<IProfileService>();
            this.profileTemplateService = serviceProvider.GetRequiredService<IProfileTemplateService>();
            this.commonService = serviceProvider.GetRequiredService<ICommonService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.domainService = serviceProvider.GetRequiredService<IStudentService>();
            this.appDbContext = serviceProvider.GetRequiredService<IAppDbContext>();
            this.branchService = serviceProvider.GetRequiredService<IBranchService>();
            this.parentService = serviceProvider.GetRequiredService<IParentService>();
            this.studentService = serviceProvider.GetRequiredService<IStudentService>();
        }
        public override async Task Validate(tbl_Student model)
        {
            if (model.branchId.HasValue)
            {
                var hasBranch = await branchService.AnyAsync(x => x.id == model.branchId);
                if (!hasBranch)
                    throw new AppException("Không tìm thấy chi nhánh");
            }
        }
        /// <summary>
        /// Lấy danh sách học viên có thể xếp lớp = học viên đã tốt nghiệp khối - 1 của năm học trước + những học viên mới
        /// </summary>
        /// <returns></returns>
        [HttpGet("available-student")]
        [AppAuthorize]
        [Description("Danh sách học viên có thể xếp lớp")]
        public async Task<AppDomainResult> AvailableStudent([FromQuery] AvailableStudentRequest request)
        {
            var data = await this.studentService.AvailableStudent(request);
            return new AppDomainResult(data);
        }

        /// <summary>
        /// Lấy danh sách tiêu chí hồ sơ
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        [HttpGet("profile/{studentId}")]
        [AppAuthorize]
        [Description("Lấy thông tin hồ sơ học viên")]
        public async Task<AppDomainResult> GetProfile(Guid studentId)
        {
            var result = new List<tbl_Profile>();
            //lấy danh sách tiêu chí hồ sơ
            var profileTemplate = await this.profileTemplateService.GetAsync(x => x.deleted == false);
            if (profileTemplate == null || profileTemplate.Count == 0)
                throw new AppException(MessageContants.nf_profileTemplate);

            foreach (var item in profileTemplate.OrderBy(x => x.index))
            {
                var profile = await profileService.GetSingleAsync(x => x.profileTemplateId == item.id && x.studentId == studentId && x.deleted == false);
                result.Add(new tbl_Profile
                {
                    studentId = studentId,
                    profileTemplateId = item.id,
                    text = profile != null ? profile.text : null,
                    option = profile != null ? profile.option : null,
                    name = item.name,
                    type = item.type.Value
                });
            }
            return new AppDomainResult(result);
        }
        /// <summary>
        /// Cập nhật thông tin hồ sơ học viên
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("profile")]
        [AppAuthorize]
        [Description("Cập nhật thông tin hồ sơ học viên")]
        public async Task<AppDomainResult> UpdateProfile([FromBody] ProfileUpdates itemModel)
        {
            using (var tran = await appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (!itemModel.Items.Any())
                        throw new AppException("Không tìm thấy dữ liệu");
                    var userId = LoginContext.Instance.CurrentUser.userId;
                    foreach (var item in itemModel.Items)
                    {
                        var hasProfileTemplate = await profileTemplateService.AnyAsync(x => x.id == item.profileTemplateId);
                        if (!hasProfileTemplate)
                            throw new AppException("Không tìm thấy thông tin hồ sơ");
                        var hasStudent = await studentService.AnyAsync(x => x.id == item.studentId);
                        if (!hasStudent)
                            throw new AppException("Không tìm thấy học viên");
                        var profile = await profileService
                            .GetSingleAsync(x => x.studentId == item.studentId && x.profileTemplateId == item.profileTemplateId && x.deleted == false);
                        if (profile == null)
                        {
                            profile = new tbl_Profile
                            {
                                active = true,
                                created = Timestamp.Now(),
                                createdBy = userId,
                                deleted = true,
                                option = item.option,
                                profileTemplateId = item.profileTemplateId,
                                studentId = item.studentId,
                                text = item.text,
                                updated = Timestamp.Now()
                            };
                            await profileService.CreateAsync(profile);
                        }
                        else
                        {
                            profile.option = item.option;
                            profile.text = item.text;
                            await profileService.UpdateAsync(profile);
                        }
                    }
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
        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] StudentCreate itemModel)
        {

            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_Student>(itemModel);
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            item.code = await this.autoGenCodeConfigService.AutoGenCode(nameof(tbl_Student));
            await Validate(item);
            await this.domainService.AddItem(item);
            return new AppDomainResult(item);
        }

        ///// <summary>
        ///// Upload ảnh đại diện
        ///// 0 - thumbnail
        ///// 1 - document
        ///// </summary>
        ///// <param name="file"></param>
        ///// <returns></returns>
        //[HttpPost("file")]
        //[AppAuthorize]
        //[Description("Upload ảnh đại diện")]
        //public async Task<string> UploadFile(IFormFile file)
        //{
        //    if (!FileValidator.IsImageFile(file))
        //        throw new AppException(MessageContants.un_suport_file);
        //    return await base.UploadFile(file, "Images");
        //}

        /// <summary>
        /// Lấy danh sách trẻ theo phụ huynh
        /// </summary>
        /// <returns></returns>
        [HttpGet("by-parent")]
        [AppAuthorize]
        [Description("Lấy danh sách trẻ theo phụ huynh")]
        public async Task<AppDomainResult> GetByParent()
        {
            var userLog = LoginContext.Instance.CurrentUser ?? throw new AppException(MessageContants.auth_expiried);
            var parent = await this.parentService.GetSingleAsync(x => x.userId == userLog.userId) ?? throw new AppException(MessageContants.nf_parent);

            var result = await studentService.GetByParent(parent.id);

            return new AppDomainResult(result);
        }

        /// <summary>
        /// Thông tin hồ sơ trẻ cho app
        /// </summary>
        /// <returns></returns>
        [HttpGet("mobile-profile")]
        [AppAuthorize]
        [Description("Thông tin hồ sơ trẻ cho app")]
        public async Task<AppDomainResult> GetProfileForMobile()
        {
            var userLog = LoginContext.Instance.CurrentUser ?? throw new AppException(MessageContants.auth_expiried);
            var parent = await this.parentService.GetSingleAsync(x => x.userId == userLog.userId) ?? throw new AppException(MessageContants.nf_parent);
            var result = await studentService.GetProfileForMobile(parent.id);
            return new AppDomainResult
            {
                resultCode = ((int)HttpStatusCode.OK),
                resultMessage = "Thành công",
                success = true,
                data = new
                {
                    items = result,
                    totalItem = result.Count,
                }
            };
        }

        [HttpGet("by-grade")]
        [AppAuthorize]
        [Description("Danh sách học sinh theo khối")]
        public async Task<AppDomainResult> GetStudentByGrade([FromQuery] GetStudentByGradeRequest request)
        {
            var data = await this.studentService.GetStudentByGrade(request);
            return new AppDomainResult(data);
        }

        /// <summary>
        /// Sổ liên lạc điện tử
        /// </summary>
        /// <returns></returns>
        [HttpGet("school-report-card")]
        [AppAuthorize]
        [Description("Số liên lạc điện tử")]
        public async Task<AppDomainResult> SchoolReportCard([FromQuery] Guid studentId)
        {
            var data = await this.studentService.SchoolReportCard(studentId);
            return new AppDomainResult(data);
        }




        /// <summary>
        /// Thêm mới nhieu hoc vien
        /// </summary>
        /// <param name="List itemModel"></param>
        /// <returns></returns>
        [HttpPost("add-list-student")]
        [AppAuthorize]
        [Description("Thêm mới Nhieu Hoc Vien")]
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
                await Validate(item);
                await this.domainService.AddItem(item);
            }
            return new AppDomainResult(itemModel);
        }


        [HttpGet("excel/export")]
        [AppAuthorize]
        [Description("Export")]
        public async Task<AppDomainResult> Export([FromQuery] StudentSearchExport search)
        {
            var fileUrl = "";
            fileUrl = await this.studentService.Export(search);
            return new AppDomainResult(fileUrl);
        }

        [HttpGet("excel/export-template")]
        [AppAuthorize]
        [Description("Export")]
        public async Task<AppDomainResult> ExportTemplate()
        {
            var fileUrl = "";
            fileUrl = await this.studentService.ExportTemplate();
            return new AppDomainResult(fileUrl);
        }
        [HttpPost("excel/import")]
        [AppAuthorize]
        [Description("Import")]
        public async Task<AppDomainResult> Import(IFormFile file)
        {

           var list = await this.studentService.Import(file);
            return new AppDomainResult(list);

        }
    }
}
