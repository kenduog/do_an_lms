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
    [Description("Màn hình chi nhánh")]
    [Authorize]
    public class BranchController : BaseController<tbl_Branch, BranchCreate, BranchUpdate, BranchSearch>
    {
        private  INecessaryService necessaryService { get; set; }
        public BranchController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Branch, BranchCreate, BranchUpdate, BranchSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<IBranchService>();
            this.necessaryService = serviceProvider.GetRequiredService<INecessaryService>();
        }
        [NonAction]
        public override async Task Validate(tbl_Branch model)
        {
            if (!string.IsNullOrEmpty(model.code))
            {
                var hasCode = await domainService.AnyAsync(x => x.code.ToUpper() == model.code.ToUpper() && x.deleted == false && x.id != model.id);
                if (hasCode)
                    throw new AppException("Mã chi nhánh đã tồn tại");
            }
        }
    }
}
