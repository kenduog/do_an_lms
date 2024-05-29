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

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Nha cung cap")]
    [Authorize]
    public class ValueEntryController : ControllerBase
    {
        private readonly IValueEntryService valueEntryService;
        public ValueEntryController(IServiceProvider serviceProvider) 
        {
            this.valueEntryService = serviceProvider.GetRequiredService<IValueEntryService>();
        }

        /// <summary>
        /// Danh sách thẻ kho theo sản phẩm
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize]
        [Description("Danh sách thẻ kho")]
        public async Task<AppDomainResult> GetValueEntry([FromQuery]ValueEntrySearch search)
        {
            var data = await this.valueEntryService.GetValueEntry(search);
            return new AppDomainResult(data);
        }


        [HttpGet("excel")]
        [AppAuthorize]
        [Description("Export")]
        public async Task<AppDomainResult> Export([FromQuery] ValueEntrySearch search)
        {
            var fileUrl = "";
            fileUrl = await this.valueEntryService.Export(search);
            return new AppDomainResult(fileUrl);
        }
    }
}
