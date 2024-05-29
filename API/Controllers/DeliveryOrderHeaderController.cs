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
    [Description("Chức vụ")]
    [Authorize]
    public class DeliveryOrderHeaderController : BaseController<tbl_DeliveryOrderHeader, DeliveryOrderHeaderCreate, DeliveryOrderHeaderUpdate, DeliveryOrderHeaderSearch>
    {
        private readonly IDeliveryOrderHeaderService deliveryOrderHeaderService;
        public DeliveryOrderHeaderController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_DeliveryOrderHeader, DeliveryOrderHeaderCreate, DeliveryOrderHeaderUpdate, DeliveryOrderHeaderSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<IDeliveryOrderHeaderService>();
            this.deliveryOrderHeaderService = serviceProvider.GetRequiredService<IDeliveryOrderHeaderService>();
        }

        [NonAction]
        public override Task<AppDomainResult> GetById(Guid id) => base.GetById(id);

        [NonAction]
        public override Task<AppDomainResult> DeleteItem(Guid id) => base.DeleteItem(id);

        /// <summary>
        /// Cập nhật trạng thái phiếu nhập kho
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("change-status")]
        [AppAuthorize]
        [Description("Cập nhật trạng thái phiếu nhập kho")]
        public async Task<AppDomainResult> UpdateStatus([FromBody] DeliveryOrderHeaderStatusUpdate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            await this.deliveryOrderHeaderService.ChangeStatus(itemModel);
            return new AppDomainResult();
        }
    }
}
