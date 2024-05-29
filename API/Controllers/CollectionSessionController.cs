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
    [Description("Chi tiết đợt thu")]
    [Authorize]
    public class CollectionSessionController : BaseController<tbl_CollectionSession, CollectionSessionCreate, CollectionSessionUpdate, CollectionSessionSearch>
    {
        private readonly ICollectionSessionService collectionSessionService;
        public CollectionSessionController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_CollectionSession, CollectionSessionCreate, CollectionSessionUpdate, CollectionSessionSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.collectionSessionService = serviceProvider.GetRequiredService<ICollectionSessionService>();
            this.domainService = serviceProvider.GetRequiredService<ICollectionSessionService>();
        }

        [NonAction]
        public override async Task<AppDomainResult> AddItem([FromBody] CollectionSessionCreate itemModel) => await base.AddItem(itemModel);
        [NonAction]
        public override async Task<AppDomainResult> GetById(Guid id) => await base.GetById(id);

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public async Task<AppDomainResult> CustomAddItem([FromBody] CollectionSessionCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            await this.collectionSessionService.CustomAddItem(itemModel);
            return new AppDomainResult();
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("fee")]
        [AppAuthorize]
        [Description("Cập nhật các khoảng phí của học viên")]
        public async Task<AppDomainResult> UpdateCollectionByStudent([FromBody] CollectionSessionLineUpdate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            await this.collectionSessionService.UpdateFeeByStudent(itemModel);
            return new AppDomainResult();
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("other")]
        [AppAuthorize]
        [Description("Cập nhật các khoản thu khác của học viên")]
        public async Task<AppDomainResult> UpdateDeduction([FromBody] CollectionSessionHeaderUpdate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            await this.collectionSessionService.UpdateOtherFee(itemModel);
            return new AppDomainResult();
        }


        /// <summary>
        /// Lấy chi tiết đợt thu
        /// </summary>
        /// <returns></returns>
        [HttpGet("by-id")]
        [AppAuthorize]
        [Description("Lấy chi tiết đợt thu")]
        public async Task<AppDomainResult> CustomGetByIdAsync([FromQuery] CollectionSessionByIdSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data=  await this.collectionSessionService.CustomGetByIdAsync(baseSearch);

            return new AppDomainResult(data);
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <returns></returns>
        [HttpGet("by-parent")]
        [AppAuthorize]
        [Description("Lấy danh sách đợt thu cho phụ huynh")]
        public async Task<AppDomainResult> GetByParent([FromQuery] CollectionSessionByParentSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.collectionSessionService.GetByParent(baseSearch);
            return new AppDomainResult(data);
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <returns></returns>
        [HttpGet("collection-header-by-parent/{id}")]
        [AppAuthorize]
        [Description("Lấy danh sách đợt thu cho phụ huynh")]
        public async Task<AppDomainResult> GetCollectionHeaderByParent(Guid id)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.collectionSessionService.GetCollectionHeaderByParent(id);
            return new AppDomainResult(data);
        }

        /// <summary>
        /// Confirm thanh toán
        /// </summary>
        /// <returns></returns>
        [HttpPost("confirm-payment")]
        [AppAuthorize]
        [Description("Confirm thanh toán")]
        public async Task<AppDomainResult> ConfirmPayment(ComfirmPayment model)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());

            await this.collectionSessionService.ConfirmPayment(model);
            
            return new AppDomainResult();
        }

        [HttpPost("notify")]
        [AppAuthorize]
        [Description("Thông báo cho tất cả phụ huynh của các bé trong đợt thu")]
        public async Task<AppDomainResult> Notify(CollectionSessionNotificationRequest request)
        {
            await collectionSessionService.SendNotification(request);
            return new AppDomainResult();
        }
    }
}
