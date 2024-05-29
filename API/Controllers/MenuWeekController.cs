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

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Lập menu theo tuần")]
    [Authorize]
    public class MenuWeekController : ControllerBase
    {
        private readonly IMenuWeekService _menuWeekService;
        public MenuWeekController(IServiceProvider serviceProvider)
        {
            this._menuWeekService = serviceProvider.GetRequiredService<IMenuWeekService>();
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public async Task<AppDomainResult> Get([FromQuery] MenuWeekSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this._menuWeekService.CustomGet(baseSearch);
            return new AppDomainResult(data);
        }

        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Add or update")]
        public async Task<AppDomainResult> AddOrUpdateItem([FromBody] MenuWeekUpdate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            await this._menuWeekService.AddOrUpdateItem(itemModel);
            return new AppDomainResult();
        }

        /// <summary>
        /// Lấy danh sách thực đơn tuần ngẫu nhiên
        /// </summary>
        /// <returns></returns>
        [HttpGet("random")]
        [AppAuthorize]
        [Description("Lấy danh sách thực đơn tuần ngẫu nhiên")]
        public async Task<AppDomainResult> GetRandomMenuWeek([FromQuery] MenuWeekRandomSearch baseSearch)
        {
            var data = await this._menuWeekService.GetRandom(baseSearch);
            return new AppDomainResult(data);
        }
    }
}
