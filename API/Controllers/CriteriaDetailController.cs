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
    [Description("Màn hình tiêu chí đánh giá")]
    [Authorize]
    public class CriteriaDetailController : BaseController<tbl_CriteriaDetail, CriteriaDetailCreate, CriteriaDetailUpdate, CriteriaDetailSearch>
    {
        private readonly ICriteriaService criteriaService;
        public CriteriaDetailController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_CriteriaDetail, CriteriaDetailCreate, CriteriaDetailUpdate, CriteriaDetailSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.criteriaService = serviceProvider.GetRequiredService<ICriteriaService>();
            this.domainService = serviceProvider.GetRequiredService<ICriteriaDetailService>();
        }
        [NonAction]
        public override async Task Validate(tbl_CriteriaDetail model)
        {
            if (model.criteriaId.HasValue)
            {
                var item = await criteriaService.AnyAsync(x => x.id == model.criteriaId);
                if (!item)
                    throw new AppException(MessageContants.nf_criteria);
            }
            if (!string.IsNullOrEmpty(model.code))
            {
                var item = await domainService.AnyAsync(x => x.id != model.id && x.code == model.code);
                if (item)
                    throw new AppException(MessageContants.exs_code);
            }
        }
    }
}
