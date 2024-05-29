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

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Phòng ban")]
    [Authorize]
    public class DepartmentController : BaseController<tbl_Departments, DepartmentCreate, DepartmentUpdate, BaseSearch>
    {
        protected IBranchService branchService;
        public DepartmentController(IServiceProvider serviceProvider
            , ILogger<BaseController<tbl_Departments, DepartmentCreate, DepartmentUpdate, BaseSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<IDepartmentService>();
            this.branchService = serviceProvider.GetRequiredService<IBranchService>();
        }

        [Description("Tạo phòng ban")]
        public override async Task<AppDomainResult> AddItem([FromBody] DepartmentCreate itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            tbl_Departments item = new tbl_Departments()
            {
                name = itemModel.name,
            };
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            success = await this.domainService.CreateAsync(item);
            if (!success)
                throw new AppException(MessageContants.err);

            appDomainResult.success = success;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = MessageContants.success;
            return appDomainResult;
        }
        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut]
        [AppAuthorize]
        [Description("Chỉnh sửa")]
        public override async Task<AppDomainResult> UpdateItem([FromBody] DepartmentUpdate itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_Departments>(itemModel);

            await Validate(item);

            if (item == null)
                throw new KeyNotFoundException(MessageContants.nf_item);
            tbl_Departments data = await this.domainService.GetByIdAsync(itemModel.id);
            if (data != null)
            {
                data.name = item.name;
                success = await this.domainService.UpdateAsync(data);
            }
            if (!success)
                throw new Exception(MessageContants.err);

            appDomainResult.success = success;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = MessageContants.success;

            return appDomainResult;
        }
    }
}
