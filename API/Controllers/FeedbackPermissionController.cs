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
using static Utilities.CoreContants;
using AppDbContext;
using Service.Services;
using System.Text;
using Interface.DbContext;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Màn hình phân quyền xử lý phản hồi")]
    [Authorize]
    public class FeedbackPermissionController : BaseController<tbl_FeedbackPermission, FeedbackPermissionCreate, FeedbackPermissionUpdate, FeedbackPermissionSearch>
    {
        public FeedbackPermissionController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_FeedbackPermission, FeedbackPermissionCreate, FeedbackPermissionUpdate, FeedbackPermissionSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IFeedbackPermissionService>();
        }
    }
}
