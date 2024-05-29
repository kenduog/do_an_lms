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
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml.Style;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Cấu hình học phí")]
    [Authorize]
    public class TuitionConfigController : BaseController<tbl_TuitionConfig, TuitionConfigCreate, TuitionConfigUpdate, TuitionConfigSearch>
    {
        private ISchoolYearService schoolYearService;
        private IGradeService gradeService;
        private IAppDbContext appDbContext;
        private ITuitionConfigService tuitionConfigService;
        private IBranchService branchService;
        private ITuitionConfigCategoryService tuitionConfigCategoryService;
        public TuitionConfigController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_TuitionConfig, TuitionConfigCreate, TuitionConfigUpdate, TuitionConfigSearch>> logger
            , IWebHostEnvironment env
            , IDomainHub hubcontext) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<ITuitionConfigService>();
            this.schoolYearService = serviceProvider.GetRequiredService<ISchoolYearService>();
            this.gradeService = serviceProvider.GetRequiredService<IGradeService>();
            this.appDbContext = serviceProvider.GetRequiredService<IAppDbContext>();
            this.tuitionConfigService = serviceProvider.GetRequiredService<ITuitionConfigService>();
            this.branchService = serviceProvider.GetRequiredService<IBranchService>();
            this.tuitionConfigCategoryService = serviceProvider.GetRequiredService<ITuitionConfigCategoryService>();
        }
        [NonAction]
        public override async Task Validate(tbl_TuitionConfig model)
        {
            if (model.tuitionConfigCategoryId.HasValue)
            {
                var tuitionConfigCategory = await tuitionConfigCategoryService.AnyAsync(x=>x.id == model.tuitionConfigCategoryId && x.deleted == false);
                if (!tuitionConfigCategory)
                    throw new AppException("Không tìm thấy loại khoản thu");
            }
        }

    }
}
