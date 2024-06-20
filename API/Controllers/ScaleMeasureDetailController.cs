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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Chi tiết đợt cân đo")]
    [Authorize]
    public class ScaleMeasureDetailController : BaseController<tbl_ScaleMeasureDetail, ScaleMeasureDetailCreate, ScaleMeasureDetailUpdate, ScaleMeasureDetailSearch>
    {
        private readonly IScaleMeasureDetailService scaleMeasureDetailService;
        private readonly IParentService parentService;
        public ScaleMeasureDetailController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_ScaleMeasureDetail, ScaleMeasureDetailCreate, ScaleMeasureDetailUpdate, ScaleMeasureDetailSearch>> logger
            , IWebHostEnvironment env
            , IDomainHub hubcontext) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<IScaleMeasureDetailService>();
            this.scaleMeasureDetailService = serviceProvider.GetRequiredService<IScaleMeasureDetailService>();
            this.parentService = serviceProvider.GetRequiredService<IParentService>();
        }


        /// <summary>
        /// Export
        /// </summary>
        /// <param name="scaleMeasureId"></param>
        /// <returns></returns>
        [HttpGet("student-list/{scaleMeasureId}")]
        [AppAuthorize]
        [Description("Export")]
        public async Task<AppDomainResult> Export(Guid scaleMeasureId)
        {
            var fileUrl = "";
            fileUrl = await this.scaleMeasureDetailService.ExportStudentList(scaleMeasureId);
            return new AppDomainResult(fileUrl);
        }

        /// <summary>
        /// Import
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("import")]
        [AppAuthorize]
        [Description("Import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return BadRequest("Missing excel file");
                }

                

                var list = await this.scaleMeasureDetailService.Import(file);

                return Ok(new AppDomainResult(list));
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("by-student")]
        [AppAuthorize]
        [Description("Get by student")]
        public async Task<AppDomainResult> GetByStudent([FromQuery] ScaleMeasureDetailSearch search)
        {
            var result = new tbl_ScaleMeasureDetail();
            var data = await this.domainService.GetPagedListData(search);
            if (data.items != null && data.items.Count > 0)
                result = data.items.FirstOrDefault();

            return new AppDomainResult(result);
        }

        /// <summary>
        /// Export Template
        /// </summary>
        /// <returns></returns>
        [HttpGet("export-template")]
        [AppAuthorize]
        [Description("Export Template")]
        public async Task<AppDomainResult> ExportTemplateWithScaleMeasureDetail([FromQuery] ScaleMeasureDetailExportSearch search)
        {
            string s = await this.scaleMeasureDetailService.ExportTemplate(search);
            return new AppDomainResult(s);
        }

        /// <summary>
        /// Export
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("export")]
        [AppAuthorize]
        [Description("Export")]
        public async Task<AppDomainResult> ExportWithScaleMeasureDetail([FromQuery] ScaleMeasureDetailExportSearch search)
        {
            string s = await this.scaleMeasureDetailService.Export(search);
            return new AppDomainResult(s);
        }
        /// <summary>
        /// Save List
        /// </summary>
        /// <param name="scaleMeasureId"></param>
        /// <returns></returns>
        [HttpPost("save-list-import/{scaleMeasureId}")]
        [AppAuthorize]
        [Description("Save List Import")]
        public async Task<AppDomainResult> SaveList([FromBody] List<ScaleMeasureDetailExport> list,[FromRoute] Guid scaleMeasureId)
        {
            await this.scaleMeasureDetailService.SaveList(list, scaleMeasureId);
            return new AppDomainResult();
        }


    }
}
