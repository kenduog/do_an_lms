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
using AppDbContext;
using Microsoft.EntityFrameworkCore;
using static Utilities.CoreContants;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Màn hình lớp học")]
    [Authorize]
    public class ClassController : BaseController<tbl_Class, ClassCreate, ClassUpdate, ClassSearch>
    {
        private readonly ISchoolYearService schoolYearService;
        private readonly ISemesterService semesterService;
        private readonly IAppDbContext appDbContext;
        private readonly IClassService classService;
        private readonly IGradeService gradeService;
        private readonly IStaffService staffService;
        private readonly IStudentInClassService studentInClassService;
        private readonly IUserService userService;
        private readonly IStudentService studentService;

        public ClassController(IServiceProvider serviceProvider
            , IAppDbContext appDbContext
            , ILogger<BaseController<tbl_Class, ClassCreate, ClassUpdate, ClassSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.appDbContext = serviceProvider.GetRequiredService<IAppDbContext>();
            this.gradeService = serviceProvider.GetRequiredService<IGradeService>();
            this.semesterService = serviceProvider.GetRequiredService<ISemesterService>();
            this.schoolYearService = serviceProvider.GetRequiredService<ISchoolYearService>();
            this.classService = serviceProvider.GetRequiredService<IClassService>();
            this.domainService = serviceProvider.GetRequiredService<IClassService>();
            this.staffService = serviceProvider.GetRequiredService<IStaffService>();
            this.studentInClassService = serviceProvider.GetRequiredService<IStudentInClassService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.studentService = serviceProvider.GetRequiredService<IStudentService>();

        }
        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] ClassCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.classService.AddClass(itemModel);
            return new AppDomainResult(data);
        }
        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut]
        [AppAuthorize]
        [Description("Chỉnh sửa")]
        public override async Task<AppDomainResult> UpdateItem([FromBody] ClassUpdate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.classService.EditClass(itemModel);
            return new AppDomainResult(data);
        }
        //[HttpPost("generate")]
        //[AppAuthorize]
        //[Description("Thêm nhiều lớp cùng lúc")]
        //public async Task<AppDomainResult> GenerateClass(MultipleClassCreate request)
        //{
        //    using (var tran = await appDbContext.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            if (!ModelState.IsValid)
        //                throw new AppException(ModelState.GetErrorMessage());

        //            var data = await this.classService.GenerateClass(request);
        //            await tran.CommitAsync();
        //            return new AppDomainResult(data);
        //        }
        //        catch (Exception e)
        //        {
        //            await tran.RollbackAsync();
        //            throw new AppException(e.Message);
        //        }
        //    }
        //}

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
            using (var tran = await appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await this.domainService.DeleteItem(id);
                    var studentInClasses = await studentInClassService.GetAsync(x => x.classId == id && x.deleted == false);
                    foreach (var item in studentInClasses)
                    {
                        await studentInClassService.DeleteAsync(item.id);
                        await studentService.UpdateStatus(item.studentId, (int)StudentStatus.Moi);
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
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public override async Task<AppDomainResult> Get([FromQuery] ClassSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            if (baseSearch == null) baseSearch = new ClassSearch();
            var userLog = LoginContext.Instance.CurrentUser;
            PrivateClassSearch privteSearch = new PrivateClassSearch
            {
                gradeId = baseSearch.gradeId,
                orderBy = baseSearch.orderBy,
                pageIndex = baseSearch.pageIndex,
                pageSize = baseSearch.pageSize,
                schoolYearId = baseSearch.schoolYearId,
                searchContent = baseSearch.searchContent,
                branchId = baseSearch.branchId,
                userId = userLog.userId
            };
            var userId = LoginContext.Instance.CurrentUser.userId;
            var user = await userService.GetByIdAsync(userId);
            if (!(await userService.IsAdmin(userId)))
            {
                privteSearch.myBranchIds = user.branchIds;
            }
            PagedList<tbl_Class> pagedData = await this.domainService.GetPagedListData(privteSearch);
            pagedData.items.ToList().ForEach(x => x.classShifts = Task.Run(() => classService.ClassShiftByClassAsync(x.id)).Result);
            return new AppDomainResult(pagedData);
        }

        /// <summary>
        /// Lấy danh sách lớp còn trống
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("class-empty")]
        [AppAuthorize]
        [Description("Lấy danh sách lớp còn trống")]
        public async Task<AppDomainResult> GetClassEmpty([FromQuery] ClassSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            if (baseSearch == null) baseSearch = new ClassSearch();
            var userLog = LoginContext.Instance.CurrentUser;
            PrivateClassSearch privteSearch = new PrivateClassSearch
            {
                gradeId = baseSearch.gradeId,
                orderBy = baseSearch.orderBy,
                pageIndex = baseSearch.pageIndex,
                pageSize = baseSearch.pageSize,
                schoolYearId = baseSearch.schoolYearId,
                searchContent = baseSearch.searchContent,
                branchId = baseSearch.branchId,
                userId = userLog.userId
            };
            var userId = LoginContext.Instance.CurrentUser.userId;
            var user = await userService.GetByIdAsync(userId);
            if (!(await userService.IsAdmin(userId)))
            {
                privteSearch.myBranchIds = user.branchIds;
            }
            PagedList<tbl_Class> pagedData = await this.classService.GetClassEmpty(privteSearch);
            pagedData.items.ToList().ForEach(x => x.classShifts = Task.Run(() => classService.ClassShiftByClassAsync(x.id)).Result);
            return new AppDomainResult(pagedData);
        }

        /// <summary>
        /// Lấy danh sách lớp cho thời khóa biểu
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("prepare-time-table")]
        [AppAuthorize]
        [Description("Lấy danh sách lớp cho thời khóa biểu")]
        public async Task<AppDomainResult> PrepareToCreateTimeTable([FromQuery] ClassPrepare baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            if (baseSearch == null) baseSearch = new ClassPrepare();
            List<ClassToPrepare> data = await this.classService.GetClassToPrepare(baseSearch);
            return new AppDomainResult(data);
        }

    }
}
