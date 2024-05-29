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
using Entities.AuthEntities;
using AppDbContext;
using Microsoft.AspNetCore.Http;
using API.Model;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Nhóm bảng tin")]
    [Authorize]
    public class GroupNewsController : BaseController<tbl_GroupNews, GroupNewsCreate, GroupNewsUpdate, BaseSearch>
    {
        private IParentService parentService;
        private IStudentService studentService;
        private IStudentInClassService studentInClassService;
        private IUserJoinGroupNewsService userJoinGroupNewsService;
        private IUserService userService;
        private IClassService classService;
        private IGroupNewsService groupNewsService;

        public GroupNewsController(IServiceProvider serviceProvider
            , ILogger<BaseController<tbl_GroupNews, GroupNewsCreate, GroupNewsUpdate, BaseSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<IGroupNewsService>();
            this.parentService = serviceProvider.GetRequiredService<IParentService>();
            this.studentService = serviceProvider.GetRequiredService<IStudentService>();
            this.userJoinGroupNewsService = serviceProvider.GetRequiredService<IUserJoinGroupNewsService>();
            this.studentInClassService = serviceProvider.GetRequiredService<IStudentInClassService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.classService = serviceProvider.GetRequiredService<IClassService>();
            this.groupNewsService = serviceProvider.GetRequiredService<IGroupNewsService>();
        }
        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] GroupNewsCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_GroupNews>(itemModel);
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            var newData = await this.domainService.AddItemWithResponse(item);
            // Thêm user vào nhóm
            bool success = await this.groupNewsService.UserJoinGroupForClass(itemModel, newData);
            if (!success)
                throw new AppException(MessageContants.err);
            var data = await this.domainService.GetByIdAsync(newData.id);
            return new AppDomainResult(data);
        }
        /// <summary>
        /// Lấy danh sách người dùng trong nhóm/ngoài nhóm 
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("user-group")]
        [AppAuthorize]
        [Description("Lấy danh sách người dùng trong nhóm/ngoài nhóm")]
        public async Task<AppDomainResult> GetUserGroupNews([FromQuery] UserJoinGroupNewsSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.groupNewsService.GetUserGroupNews(baseSearch);
            var newData = data.items.Select(x => new MemberGroupNewsDTO()
            {
                userId = x.id,
                userCode = x.code,
                birthday = x.birthday,
                email = x.email,
                firstName = x.firstName,
                lastName = x.lastName,
                fullName = x.fullName,
                gender = x.gender,
                genderName = x.genderName,
                isSuperUser = x.isSuperUser,
                phone = x.phone,
                status = x.status,
                statusName = x.statusName,
                thumbnail = x.thumbnail,
                username = x.username,
                typeUserGroup = Task.Run(() => userJoinGroupNewsService.GetTypeUser(baseSearch.groupNewsId, x.id)).Result,
                typeUserGroupName = Task.Run(() => userJoinGroupNewsService.GetTypeUserName(baseSearch.groupNewsId, x.id)).Result,
                groupName = Task.Run(() => userJoinGroupNewsService.GetGroupName(x.id)).Result,
            }).ToList();
            int totalPage = 0;
            decimal count = data.totalItem;
            if (count > 0)
                totalPage = (int)Math.Ceiling(count / baseSearch.pageSize);
            return new AppDomainResult
            {
                resultCode = ((int)HttpStatusCode.OK),
                resultMessage = "Thành công",
                success = true,
                data = new
                {
                    items = newData,
                    pageIndex = baseSearch.pageIndex,
                    pageSize = baseSearch.pageSize,
                    totalItem = data.totalItem,
                    totalPage = totalPage
                }
            };
        }
        /// <summary>
        /// Cấp quyền quản trị viên cho thành viên trong nhóm
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("grant-permissions")]
        [AppAuthorize]
        [Description("Cấp quyền quản trị viên cho thành viên trong nhóm")]
        public async Task<AppDomainResult> GrantPermissions([FromBody] GrantPermissionsGroupNewsCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var newData = await this.groupNewsService.GrantPermissions(itemModel);
            return new AppDomainResult(newData);
        }
        /// <summary>
        /// Chuyển chủ nhóm
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("change-owner")]
        [AppAuthorize]
        [Description("Chuyển chủ nhóm")]
        public async Task<AppDomainResult> ChangeOwner([FromBody] ChangeOwnerGroupNewsCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var newData = await this.groupNewsService.ChangeOwner(itemModel);
            return new AppDomainResult(newData);
        }
        /// <summary>
        /// Hủy cấp quyền
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("revoke-permissions")]
        [AppAuthorize]
        [Description("Hủy cấp quyền")]
        public async Task<AppDomainResult> RevokePermissions([FromBody] ChangeOwnerGroupNewsCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var newData = await this.groupNewsService.RevokePermissions(itemModel);
            return new AppDomainResult(newData);
        }
        /// <summary>
        /// Kiểm tra admin trong nhóm
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost("check-admin")]
        [AppAuthorize]
        [Description("Kiểm tra admin trong nhóm")]
        public async Task<AppDomainResult> CheckAdmin([FromBody] CheckAdminGroupNewsCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var newData = await this.groupNewsService.CheckAdmin(itemModel);
            return new AppDomainResult(newData);
        }
    }
}
