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

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Phân bổ người dùng vào nhóm")]
    [Authorize]
    public class UserGroupController : BaseController<tbl_UserGroup, UserGroupCreate, UserGroupUpdate, BaseSearch>
    {
        public UserGroupController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_UserGroup, UserGroupCreate, UserGroupUpdate, BaseSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<IUserGroupService>();
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] UserGroupCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var hasGroup = await domainService
                .AnyAsync(x => x.userId == itemModel.userId && x.groupId == itemModel.groupId && x.deleted == false);
            if (hasGroup)
                throw new AppException("Người dùng đã có nhóm quyền này");
            var item = mapper.Map<tbl_UserGroup>(itemModel);
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            await Validate(item);
            await this.domainService.AddItem(item);
            return new AppDomainResult(item);
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
        public override async Task<AppDomainResult> UpdateItem([FromBody] UserGroupUpdate itemModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new AppException(ModelState.GetErrorMessage());
                var item = mapper.Map<tbl_UserGroup>(itemModel);

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
        /// <summary>
        /// Xóa item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpDelete]
        [AppAuthorize]
        [Description("Xoá nhóm quyền người dùng")]
        public async Task<AppDomainResult> DeleteItem([FromBody] UserGroupDelete itemModel)
        {
            try
            {
                var item = await domainService.GetSingleAsync(x => x.userId == itemModel.userId && x.groupId == itemModel.groupId && x.deleted == false);
                if (item == null)
                    throw new AppException("Không tìm thấy dữ liệu");
                await this.domainService.DeleteItem(item.id);
                return new AppDomainResult();
            }
            catch (AppException e)
            {
                throw new AppException(e.Message);
            }
        }
    }
}
