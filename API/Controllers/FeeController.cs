﻿using Entities;
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
    [Description("Các khoản cần thu")]
    [Authorize]
    public class FeeController : BaseController<tbl_Fee, FeeCreate, FeeUpdate, FeeSearch>
    {
        private readonly IFeeService feeService;
        public FeeController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Fee, FeeCreate, FeeUpdate, FeeSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<IFeeService>();
            this.feeService = serviceProvider.GetRequiredService<IFeeService>();
        }

        [HttpGet("by-collection")]
        [AppAuthorize]
        [Description("Danh sách khoản thu theo kế hoạch thu")]
        public async Task<AppDomainResult> GetFeeByCollectionPlan([FromQuery] GetFeeByCollectionPlanRequest request)
        {
            var data = await this.feeService.GetFeeByCollectionPlan(request);
            return new AppDomainResult(data);
        }
    }
}
