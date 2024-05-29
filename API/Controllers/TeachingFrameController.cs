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
    [Description("Khung chương trình dạy học")]
    [Authorize]
    public class TeachingFrameController : BaseController<tbl_TeachingFrame, TeachingFrameCreate, TeachingFrameUpdate, TeachingFrameSearch>
    {
        private readonly IBranchService branchService;
        private readonly ITeachingFrameService teachingFrameService;
        public TeachingFrameController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_TeachingFrame, TeachingFrameCreate, TeachingFrameUpdate, TeachingFrameSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.branchService = serviceProvider.GetRequiredService<IBranchService>();
            this.domainService = serviceProvider.GetRequiredService<ITeachingFrameService>();
            this.teachingFrameService = serviceProvider.GetRequiredService<ITeachingFrameService>();
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("grade")]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public async Task<AppDomainResult> GetByGrade([FromQuery] TeachingFrameGroupByGradeSearch baseSearch)
        {
            var data = await this.teachingFrameService.GetTeachingFrameGroupByGrade(baseSearch);
            return new AppDomainResult(data);
        }
    }
}
