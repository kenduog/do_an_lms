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
using Microsoft.Identity.Client;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Phân bổ quyền cho nhóm")]
    [Authorize]
    public class GroupPermissionController : BaseController<tbl_GroupPermission, GroupPermissionCreate, GroupPermissionUpdate, BaseSearch>
    {
        private readonly IAppDbContext appDbContext;
        private readonly IGroupService groupService;
        private readonly IPermissionService permissionService;
        private readonly IGroupPermissionService groupPermissionService;
        private readonly IContentTypeService contentTypeService;
        public GroupPermissionController(IServiceProvider serviceProvider
            , ILogger<BaseController<tbl_GroupPermission, GroupPermissionCreate, GroupPermissionUpdate, BaseSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.groupPermissionService = serviceProvider.GetRequiredService<IGroupPermissionService>();
            this.contentTypeService = serviceProvider.GetRequiredService<IContentTypeService>();
            this.appDbContext = serviceProvider.GetRequiredService<IAppDbContext>();
            this.domainService = serviceProvider.GetRequiredService<IGroupPermissionService>();
            this.permissionService = serviceProvider.GetRequiredService<IPermissionService>();
            this.groupService = serviceProvider.GetRequiredService<IGroupService>();
        }

        [NonAction]
        public override async Task Validate(tbl_GroupPermission model)
        {
            if (model.groupId.HasValue)
            {
                var group = await this.groupService.AnyAsync(x => x.deleted == false && x.id == model.groupId.Value);
                if(!group)
                    throw new AppException(MessageContants.nf_group);
            }
            if (model.permissionId.HasValue)
            {
                var permission = await this.permissionService.AnyAsync(x => x.deleted == false && x.id == model.permissionId.Value);
                if (!permission)
                    throw new AppException(MessageContants.nf_permission);
            }
        }

        [HttpPut("grant-all")]
        [AppAuthorize]
        public async Task<AppDomainResult> GrantAllPermissionToGroup([FromBody] DomainUpdate item)
        {
            var userLog = LoginContext.Instance.CurrentUser ?? throw new AppException(MessageContants.auth_expiried);
            if (!userLog.isSuperUser)
                throw new AppException(MessageContants.feature_for_super_user);

            await this.groupPermissionService.GrantAllPermissionToGroup(item);
            return new AppDomainResult();
        }

        [HttpPut("remove-all")]
        [AppAuthorize]
        public async Task<AppDomainResult> RemoveAllPermissionFromGroup([FromBody] DomainUpdate item)
        {
            var userLog = LoginContext.Instance.CurrentUser ?? throw new AppException(MessageContants.auth_expiried);
            if (!userLog.isSuperUser)
                throw new AppException(MessageContants.feature_for_super_user);

            await this.groupPermissionService.RemoveAllPermissionToGroup(item);
            return new AppDomainResult();
        }
    }
}
