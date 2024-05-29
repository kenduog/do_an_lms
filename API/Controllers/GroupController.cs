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
using static Utilities.CoreContants;
using OfficeOpenXml.Core.Worksheet.Fill;
using System.Linq.Dynamic.Core;
using Service.Services;
using Entities.AuthEntities;
using Entities.DomainEntities;
using BaseAPI.Controllers;
using System.Net.Mime;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Description("Màn hình quản lý quyền")]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class GroupController : BaseController<tbl_Group, GroupCreate, GroupUpdate, BaseSearch>
    {
        protected IGroupService roleService;
        protected IContentTypeService contentTypeDetailService;
        protected IPermissionService permissionService;

        public GroupController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Group, GroupCreate, GroupUpdate, BaseSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<IGroupService>();
            this.roleService = serviceProvider.GetRequiredService<IGroupService>();
            this.permissionService = serviceProvider.GetRequiredService<IPermissionService>();
            this.contentTypeDetailService = serviceProvider.GetRequiredService<IContentTypeService>();
        }
        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] GroupCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_Group> (itemModel);
            var hasCode = await domainService.AnyAsync(x => x.code.ToUpper() == item.code.ToUpper() && x.deleted == false);
            if (hasCode)
                throw new AppException("Mã nhóm quyền đã tồn tại");
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            await Validate(item);
            await this.domainService.AddItem(item);
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
        public override async Task<AppDomainResult> UpdateItem([FromBody] GroupUpdate itemModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new AppException(ModelState.GetErrorMessage());

                var group = await domainService.GetByIdAsync(itemModel.id);
                if (group == null)
                    throw new AppException("Không tìm thấy nhóm quyền");
                if (group.code == "QTV" || group.code == "GV" || group.code == "PH")
                    throw new AppException($"Không thể cập nhật nhóm quyền {group.name}");

                var hasCode = await domainService.AnyAsync(x => x.code.ToUpper() == itemModel.code.ToUpper() && x.deleted == false && x.id != itemModel.id);
                if (hasCode)
                    throw new AppException("Mã nhóm quyền đã tồn tại");

                var item = mapper.Map<tbl_Group>(itemModel);
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
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AppAuthorize]
        [Description("Xoá")]
        public override async Task<AppDomainResult> DeleteItem(Guid id)
        {
            try
            {
                var group = await domainService.GetByIdAsync(id);
                if (group == null)
                    throw new AppException("Không tìm thấy nhóm quyền");
                if (group.code == "Admin" || group.code == "Teacher" || group.code == "Parent")
                    throw new AppException($"Không thể cập nhật nhóm quyền {group.name}");
                await this.domainService.DeleteItem(id);
                return new AppDomainResult();
            }
            catch (AppException e)
            {
                throw new AppException(e.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách màn hình
        /// </summary>
        /// <param name="groupCode"></param>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        [HttpGet("permission/menu/{groupCode}")]
        [AllowAnonymous]
        public async Task<AppDomainResult> GetController(string groupCode)
        {
            List<tbl_ContentType> menu = new List<tbl_ContentType>();
            var userLog = LoginContext.Instance.CurrentUser ?? throw new AppException(MessageContants.auth_expiried);
            var contentTypeDetails = await this.contentTypeDetailService.GetMenu(userLog.userId, userLog.isSuperUser, groupCode);

            if (contentTypeDetails == null || contentTypeDetails.Count == 0)
                return new AppDomainResult(menu);

            menu = contentTypeDetails
                .Where(x=>x.isRoot == true)
                .Select(x => new tbl_ContentType { id = x.id, code = x.code, name = x.name, route = x.route, isRoot = x.isRoot})
                .Distinct()
                .ToList();

            var result = GetChild(menu, contentTypeDetails);

            //foreach(var contentType in menu.ToList())
            //{
            //    if (contentType.childs == null || contentType.childs.Count == 0)
            //        menu.Remove(contentType);
            //}

            return new AppDomainResult(result);
        }

        private List<tbl_ContentType> GetChild(List<tbl_ContentType> parents, List<tbl_ContentType> source)
        {
            if (parents == null || parents.Count == 0)
                return null;
            foreach(var parent in parents)
            {
                var details = source.Where(x => x.parentId == parent.id).ToList();
                parent.childs = GetChild(details, source);
            }
            return parents;
        }

        /// <summary>
        /// Lấy danh sách chức năng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("permission/controller-action")]
        public async Task<AppDomainResult> GetAction([FromQuery] GetControllerActionByDevWithGroupCode request)
        {
            var userLog = LoginContext.Instance.CurrentUser ?? throw new AppException(MessageContants.auth_expiried);
            var actions = await this.permissionService.GetAction(request.code, userLog.userId, userLog.isSuperUser, request.selectedGroupCode);

            List<string> data = new List<string>();
            if (actions.Any())
                data = actions.Select(x => x.code).ToList();
            return new AppDomainResult(data);
        }
        public class GetControllerActionByDevWithGroupCode
        {
            public string code { get; set; }
            public string selectedGroupCode { get; set; }
        }
    }
}
