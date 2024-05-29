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
using Entities.AuthEntities;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using ControllerModel = API.Model.ControllerModel;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Cấu hình menu")]
    [Authorize]
    public class ContentTypeController : BaseController<tbl_ContentType, ContentTypeCreate, ContentTypeUpdate, ContentTypeSearch>
    {
        private readonly IAppDbContext appDbContext;
        private readonly IPermissionService permissionService;
        private readonly IContentTypeService contentTypeService;
        public ContentTypeController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_ContentType, ContentTypeCreate, ContentTypeUpdate, ContentTypeSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.permissionService = serviceProvider.GetRequiredService<IPermissionService>();
            this.contentTypeService = serviceProvider.GetRequiredService<IContentTypeService>();
            this.domainService = serviceProvider.GetRequiredService<IContentTypeService>();
            this.appDbContext = serviceProvider.GetRequiredService<IAppDbContext>();
        }
        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public override async Task<AppDomainResult> Get([FromQuery] ContentTypeSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.contentTypeService.GetData(baseSearch);
            return new AppDomainResult(data);
        }

        /// <summary>
        /// Lấy danh sách toàn bộ menu theo group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet("group/{groupId}")]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public async Task<AppDomainResult> GetAllByGroup(Guid groupId)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.contentTypeService.GetMenuByGroup(groupId);
            return new AppDomainResult(data);
        }

        /// <summary>
        /// Lấy danh sách tất cả menu
        /// </summary>
        /// <returns></returns>
        [HttpGet("all-menu")]
        [AppAuthorize]
        [Description("Lấy danh sách tất cả menu")]
        public async Task<AppDomainResult> GetAllMenu()
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.contentTypeService.GetAllMenu();
            return new AppDomainResult(data);
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] ContentTypeCreate itemModel)
        {
            using (var tran = await appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (!ModelState.IsValid)
                        throw new AppException(ModelState.GetErrorMessage());
                    var item = mapper.Map<tbl_ContentType>(itemModel);
                    if (item == null)
                        throw new AppException(MessageContants.nf_item);

                    //validate
                    var validCode = await this.domainService.AnyAsync(x => x.code == itemModel.code && x.deleted == false);
                    if(validCode)
                        throw new AppException(MessageContants.exs_groupCode);
                    await this.domainService.CreateAsync(item);

                    //Nếu thêm menu con cho 1 menu thì tạo sẵn 1 record trong bảng permission -> tránh phải đệ quy lúc lấy menu
                    if (item.parentId.HasValue)
                    {
                        var parent = await this.domainService.GetByIdAsync(item.parentId.Value) ?? throw new AppException(MessageContants.nf_contentType);
                        var permission = await this.permissionService.GetSingleAsync(x => x.deleted == false && x.contentTypeId == parent.id);
                        if (permission == null)
                        {
                            await this.permissionService.CreateAsync(new tbl_Permission
                            {
                                contentTypeId = parent.id,
                                code = parent.code,
                                isParent = true
                            });
                        }
                    }
                    await tran.CommitAsync();

                    return new AppDomainResult();
                }
                catch (Exception e)
                {
                    await tran.RollbackAsync();
                    throw new AppException(e.Message);
                }
            }
        }

        /// <summary>
        /// Lấy danh sách controller-action
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        [HttpGet("controller-action")]
        [AppAuthorize]
        [Description("Lấy danh sách controller-action")]
        public AppDomainResult GetActions()
        {
            System.AppDomain currentDomain = System.AppDomain.CurrentDomain;
            Assembly[] assems = currentDomain.GetAssemblies();
            var controllers = new List<ControllerModel>();
            foreach (Assembly assem in assems)
            {
                var controller = assem.GetTypes().Where(type =>
                typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract)
                  .Select(e => new API.Model.ControllerModel()
                  {
                      id = e.Name.Replace("Controller", string.Empty),
                      name = string.Format("{0}", ReflectionUtilities.GetClassDescription(e)).Replace("Controller", string.Empty),
                      actions = (from i in e.GetMembers().Where(x => (
                                 x.Module.Name == "API.dll" ||
                                 x.Module.Name == "BaseAPI.dll")
                                 && x.Name != ".ctor"
                                 && x.Name != "Validate"
                                 && x.Name != "UploadFile"
                                 && x.GetCustomAttributes(typeof(NonActionAttribute)).Select(x => x.GetType().Name).FirstOrDefault() != typeof(NonActionAttribute).Name
                                 )
                                 select new API.Model.ActionModel
                                 {
                                     id = $"{e.Name.Replace("Controller", string.Empty)}-{i.Name}",
                                     name = i.GetCustomAttributes(typeof(DescriptionAttribute), true)
                                                 .Cast<DescriptionAttribute>().Select(d => d.Description)
                                                 .SingleOrDefault() ?? i.Name
                                 }).OrderBy(d => d.name).ToList()
                  })
                  .Where(e => e.id != "Role" && e.id != "Auth" && e.id != "Home" )
                  .OrderBy(e => e.name)
                  .Distinct();

                controllers.AddRange(controller);
            }
            return new AppDomainResult
            {
                data = controllers.SelectMany(x=>x.actions).ToList(),
                success = true,
                resultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách entitiy
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        [HttpGet("entities")]
        [AppAuthorize]
        [Description("Lấy danh sách entitiy")]
        public AppDomainResult GetEntities()
        {
            Type dbContextType = appDbContext.GetType();
            PropertyInfo[] properties = dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            List<string> dbSetNames = properties
                .Where(prop => prop.PropertyType.IsGenericType &&
                               prop.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(prop => prop.Name)
                .ToList();

            return new AppDomainResult
            {
                data = dbSetNames,
                success = true,
                resultCode = (int)HttpStatusCode.OK
            };
        }

    }
}
