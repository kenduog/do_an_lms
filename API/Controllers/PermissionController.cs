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
    [Description("Màn hình quyền")]
    [Authorize]
    public class PermissionController : BaseController<tbl_Permission, PermissionCreate, PermissionUpdate, PermissionSearch>
    {
        private readonly IContentTypeService contentTypeService;
        public PermissionController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Permission, PermissionCreate, PermissionUpdate, PermissionSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.contentTypeService = serviceProvider.GetRequiredService<IContentTypeService>();
            this.domainService = serviceProvider.GetRequiredService<IPermissionService>();
        }
        [NonAction]
        public override async Task Validate(tbl_Permission model)
        {
            if (model.contentTypeId.HasValue)
            {
                var item = await contentTypeService
                    .AnyAsync(x => x.id == model.contentTypeId);
                if (!item)
                    throw new AppException(MessageContants.nf_contentType);
            }
            var validateCode = await domainService.AnyAsync(x => x.id != model.id && x.code == model.code);
            if (validateCode)
                throw new AppException(MessageContants.exs_permissionCode);
        }
    }
}
