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
using System.Security.Cryptography.Xml;
using API.Model;
namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Miễn giảm học phí")]
    [Authorize]
    public class DiscountController : BaseController<tbl_Discount, DiscountCreate, DiscountUpdate, BaseSearch>
    {
        private ITuitionConfigService tuitionConfigService;
        public DiscountController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Discount, DiscountCreate, DiscountUpdate, BaseSearch>> logger
            , IWebHostEnvironment env
            , IDomainHub hubcontext) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<IDiscountService>();
            tuitionConfigService = serviceProvider.GetRequiredService<ITuitionConfigService>();
        }

        [NonAction]
        public override async Task Validate(tbl_Discount model)
        {
            if (model.type == 2 && model.value > 100)
                throw new AppException("Không thể giảm trên 100% giá trị khoản thu");
            if (model.tuitionConfigId.HasValue)
            {
                var hasTuitionConfig = await tuitionConfigService.AnyAsync(x => x.id == model.tuitionConfigId);
                if (!hasTuitionConfig)
                    throw new AppException("Không tìm thấy khoản thu");
            }
        }
    }
}
