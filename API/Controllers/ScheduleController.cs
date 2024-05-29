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
    [Description("Lịch học theo lớp")]
    [Authorize]
    public class ScheduleController : BaseController<tbl_Schedule, ScheduleCreate, ScheduleUpdate, BaseSearch>
    {
        private readonly IScheduleService scheduleService;
        public ScheduleController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Schedule, ScheduleCreate, ScheduleUpdate, BaseSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.scheduleService = serviceProvider.GetRequiredService<IScheduleService>();
            this.domainService = serviceProvider.GetRequiredService<IScheduleService>();
        }

        [NonAction]
        public override Task<AppDomainResult> Get([FromQuery] BaseSearch baseSearch)
        {
            return base.Get(baseSearch);
        }

        /// <summary>
        /// Lấy danh sách item không có phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public async Task<AppDomainResult> Get([FromQuery] ScheduleSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.scheduleService.GetOrGenerateSchedule(baseSearch);
            var timeTable = await this.scheduleService.GetTimeTable(baseSearch.classId ?? Guid.Empty);
            return new AppDomainResult(new
            {
                timeTableId = timeTable.id,
                items = data
            });
        }

        [HttpGet("daily")]
        [AppAuthorize]
        [Description("Lấy chi danh sách các hoạt động trong ngày của trẻ")]
        public async Task<AppDomainResult> StudentDailyActivity([FromQuery] StudentDailyActivityRequest request)
        {
            var data = await scheduleService.DailyActivity(request);
            return new AppDomainResult(data);
        }

        [HttpPost("notify")]
        [AppAuthorize]
        [Description("Thông báo cho tất cả phụ huynh của các bé trong lớp")]
        public async Task<AppDomainResult> Notify(ScheduleNotificationRequest request)
        {
            await scheduleService.SendScheduleNotification(request);
            return new AppDomainResult();
        }
    }
}
