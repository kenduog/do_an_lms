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
using AppDbContext;
using Service.Services;
using Interface.DbContext;
using System.Net.WebSockets;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Cấu hình thời gian học")]
    [Authorize]
    public class StudyShiftController : BaseController<tbl_StudyShift, StudyShiftCreate, StudyShiftUpdate, StudyShiftSearch>
    {
        private readonly IStudyShiftService studyShiftService; 
        public StudyShiftController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_StudyShift, StudyShiftCreate, StudyShiftUpdate, StudyShiftSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.studyShiftService = serviceProvider.GetRequiredService<IStudyShiftService>();
            this.domainService = serviceProvider.GetRequiredService<IStudyShiftService>();
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] StudyShiftCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());

            //await this.studyShiftService.AddRangeAsync(itemModel);

            return new AppDomainResult();
        }
    }
}
