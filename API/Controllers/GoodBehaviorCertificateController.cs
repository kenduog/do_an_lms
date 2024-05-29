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
using Entities.AuthEntities;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Phiếu bé ngoan")]
    [Authorize]
    public class GoodBehaviorCertificateController : BaseController<tbl_GoodBehaviorCertificate, DomainCreate,GoodBehaviorCertificateUpdate, GoodBehaviorCertificateSearch>
    {
        private readonly IGoodBehaviorCertificateService goodBehaviorCertificateService;
        private readonly IParentService parentService;
        private readonly IUserService userService;
        private readonly ISendNotificationService sendNotificationService;
        public GoodBehaviorCertificateController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_GoodBehaviorCertificate, DomainCreate, GoodBehaviorCertificateUpdate, GoodBehaviorCertificateSearch>> logger
            , IWebHostEnvironment env
            , IDomainHub hubcontext) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<IGoodBehaviorCertificateService>();
            this.goodBehaviorCertificateService = serviceProvider.GetRequiredService<IGoodBehaviorCertificateService>();
            this.parentService = serviceProvider.GetRequiredService<IParentService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.sendNotificationService = serviceProvider.GetRequiredService<ISendNotificationService>();
        }
        [NonAction]
        public override Task<AppDomainResult> Get([FromQuery] GoodBehaviorCertificateSearch baseSearch)
        {
            return base.Get(baseSearch);
        }
        [NonAction]
        public override Task<AppDomainResult> GetById(Guid id)
        {
            return base.GetById(id);
        }
        [NonAction]
        public override Task<AppDomainResult> DeleteItem(Guid id)
        {
            return base.DeleteItem(id);
        }
        [NonAction]
        public override Task<AppDomainResult> AddItem([FromBody] DomainCreate itemModel)
        {
            return base.AddItem(itemModel);
        }
        [NonAction]
        public override Task<AppDomainResult> UpdateItem([FromBody] GoodBehaviorCertificateUpdate itemModel)
        {
            return base.UpdateItem(itemModel);
        }

        /// <summary>
        /// Lấy danh sách item 
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public async Task<AppDomainResult> GetOrGenerateGoodBehaviorCertificate([FromQuery] GoodBehaviorCertificateSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.goodBehaviorCertificateService.GetOrGenerateGoodBehaviorCertificate(baseSearch);
            return new AppDomainResult(data);
        }

        /// <summary>
        /// Cập nhật phiếu bé ngoan
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut]
        [AppAuthorize]
        [Description("Cập nhật phiếu bé ngoan")]
        public async Task<AppDomainResult> UpdateGoodBehaviorCertificate([FromBody] List<GoodBehaviorCertificateUpdate> itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            foreach (var item in itemModel)
            {
                var data = mapper.Map<tbl_GoodBehaviorCertificate>(item);
                await this.goodBehaviorCertificateService.UpdateAsync(data);
            }
            return new AppDomainResult();
        }

        [HttpGet("learning-result")]
        [AppAuthorize]
        [Description("Lấy thông tin phiếu bé ngoan và điểm danh của bé theo tuần")]
        public async Task<AppDomainResult> GetOrGenerateGoodBehaviorCertificate([FromQuery] WeekReportRequest baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.goodBehaviorCertificateService.GetByWeek(baseSearch);
            return new AppDomainResult(data);
        }


        [HttpGet("send-notification")]
        [AppAuthorize]
        [Description("Gửi thông báo phiếu bé ngoan")]
        public async Task<AppDomainResult> SendNotiGoodBehaviorCertificate([FromQuery] GoodBehaviorCertificateSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var goodBehaviorCertificateList = await this.goodBehaviorCertificateService.GetForNoti(baseSearch);
            List<IDictionary<string, string>> notiParamList = new List<IDictionary<string, string>>();
            List<tbl_Users> receiverList = new List<tbl_Users>();
            foreach (var item in goodBehaviorCertificateList)
            {
                IDictionary<string,string> notiParam  = new Dictionary<string, string>();
                notiParam.Add("[StudentName]", item.studentName);
                notiParam.Add("[ClassName]", item.className);
                notiParam.Add("[WeekName]", item.weekName);
                if(item.fatherId != null)
                {
                    var parent = await parentService.GetByIdAsync(item.fatherId);
                    if (parent != null)
                    {
                        var parentUser = await userService.GetByIdAsync(parent.userId.Value);
                        receiverList.Add(parentUser);
                        notiParamList.Add(notiParam);
                    }
                }
                if(item.motherId != null)
                {
                    var parent = await parentService.GetByIdAsync(item.motherId);
                    if (parent != null)
                    {
                        var parentUser = await userService.GetByIdAsync(parent.userId.Value);
                        receiverList.Add(parentUser);
                        notiParamList.Add(notiParam);
                    }
                }
                if(item.guardianId != null)
                {
                    var parent = await parentService.GetByIdAsync(item.guardianId);
                    if (parent != null)
                    {
                        var parentUser = await userService.GetByIdAsync(parent.userId.Value);
                        receiverList.Add(parentUser);
                        notiParamList.Add(notiParam);
                    }
                }
            }
            string subLink = string.Empty;
            sendNotificationService.SendNotification(Guid.Parse("8142bcb0-6fce-4e01-f062-08dc173029b1"), receiverList, notiParamList, notiParamList, null, null, LookupConstant.ScreenCode_GoodBehavior, subLink);
            return new AppDomainResult();
        }


        [HttpPost("notify")]
        [AppAuthorize]
        [Description("Thông báo cho tất cả phụ huynh của các bé trong lớp")]
        public async Task<AppDomainResult> Notify(GoodBehaviorNotificationRequest request)
        {
            await goodBehaviorCertificateService.SendNotification(request);
            return new AppDomainResult();
        }
    }
}
