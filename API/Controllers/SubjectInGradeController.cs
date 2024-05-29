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
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Màn hình năm học")]
    [Authorize]
    public class SubjectInGradeController : BaseController<tbl_SubjectInGrade, SubjectInGradeCreate, SubjectInGradeUpdate, SubjectInGradeSearch>
    {
        private readonly ISubjectInGradeService subjectInGradeService;
        public SubjectInGradeController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_SubjectInGrade, SubjectInGradeCreate, SubjectInGradeUpdate, SubjectInGradeSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.subjectInGradeService = serviceProvider.GetRequiredService<ISubjectInGradeService>();
            this.domainService = serviceProvider.GetRequiredService<ISubjectInGradeService>();
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] SubjectInGradeCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());

            await this.subjectInGradeService.AddRangeAsync(itemModel);
            
            return new AppDomainResult();
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public override async Task<AppDomainResult> Get([FromQuery] SubjectInGradeSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            List<tbl_SubjectInGrade> pagedData = await this.subjectInGradeService.GetAllSubjectInGrade(baseSearch);
            return new AppDomainResult(pagedData);
        }
    }
}
