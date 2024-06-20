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
using Azure.Core;
using DocumentFormat.OpenXml.Office2016.Excel;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Màn hình điểm danh")]
    [Authorize]
    public class AttendanceController : BaseController<tbl_Attendance, AttendanceCreate, AttendanceUpdate, BaseSearch>
    {
        private readonly IAttendanceService attendanceService;
        private readonly IDayOfWeekService dayOfWeekService;
        private readonly IHolidayService holidayService;
        private readonly IClassService classService;
        private readonly IStudentService studentService;
        public AttendanceController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Attendance, AttendanceCreate, AttendanceUpdate, BaseSearch>> logger
            , IWebHostEnvironment env
            , IDomainHub hubcontext) : base(serviceProvider, logger, env
            )
        {
            this.attendanceService = serviceProvider.GetRequiredService<IAttendanceService>();
            this.domainService = serviceProvider.GetRequiredService<IAttendanceService>();
            this.dayOfWeekService = serviceProvider.GetRequiredService<IDayOfWeekService>();
            this.holidayService = serviceProvider.GetRequiredService<IHolidayService>();
            this.classService = serviceProvider.GetRequiredService<IClassService>();
            this.studentService = serviceProvider.GetRequiredService<IStudentService>();
        }
        [NonAction]
        public override Task<AppDomainResult> Get([FromQuery] BaseSearch baseSearch)
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
        public override Task<AppDomainResult> AddItem([FromBody] AttendanceCreate itemModel)
        {
            return base.AddItem(itemModel);
        }

        /// <summary>
        /// Lấy danh sách item không có phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public async Task<AppDomainResult> Get([FromQuery] AttendanceSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.attendanceService.GetOrGenerateAttendance(baseSearch);
            return new AppDomainResult(data);
        }

        [HttpPost("notify")]
        [AppAuthorize]
        [Description("Thông báo cho tất cả phụ huynh của các bé trong lớp")]
        public async Task<AppDomainResult> Notify(AttendanceNotificationRequest request)
        {
            await attendanceService.SendNotification(request);
            return new AppDomainResult();
        }

        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("update-range")]
        [AppAuthorize]
        [Description("Cập nhật điểm danh cho lớp")]
        public virtual async Task<AppDomainResult> UpdateRangeItem([FromBody] AttendanceRangeUpdate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            // Kiểm tra ngày nghỉ 
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds((long)itemModel.date);
            DateTime date = dateTimeOffset.DateTime.ToLocalTime();
            int key = (int)date.DayOfWeek + 1;
            var dayOfWeek = await dayOfWeekService.GetByKeyAsync(key)
                ?? throw new AppException(MessageContants.nf_dayOfWeek);
            var holiday = await holidayService.CheckHoliday(itemModel.date);
            if (!dayOfWeek.active.Value)
                throw new AppException(MessageContants.today_day_of_week_not_attendance);
            if (holiday)
                throw new AppException(MessageContants.today_holiday_not_attendance);
            foreach (var model in itemModel.dataUpdate)
            {
                var item = mapper.Map<tbl_Attendance>(model);

                if (item == null)
                    throw new KeyNotFoundException(MessageContants.nf_item);

                await this.domainService.UpdateItemWithResponse(item);
            }
            return new AppDomainResult
            {
                success = true,
                resultCode = (int)HttpStatusCode.OK,
                resultMessage = MessageContants.success
            };
        }
    }
}
