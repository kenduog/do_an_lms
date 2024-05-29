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
    [Description("Màn hình thông báo")]
    [Authorize]
    public class NotificationController : BaseController<tbl_Notification, NotificationCreate, NotificationUpdate, NotificationSearch>
    {
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private IDomainHub _hubContext;
        public NotificationController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Notification, NotificationCreate, NotificationUpdate, NotificationSearch>> logger
            , IWebHostEnvironment env
            , IDomainHub hubcontext) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<INotificationService>();
            this._notificationService = serviceProvider.GetRequiredService<INotificationService>();
            this._userService = serviceProvider.GetRequiredService<IUserService>();
            this._hubContext = hubcontext;
        }

        public override async Task Validate(tbl_Notification model)
        {
            var role = await _userService.GetByIdAsync(model.userId.Value);
            if (role == null)
                throw new AppException("Người dùng không tồn tại");
        }

        async void SendNotification(Guid uid)
        {
            //send notification to user
            var noti = await _notificationService.GetNotificationToSend(uid);
            var method = CoreContants.SignalRMethod.onReceiveListNotification.ToString();
            string userId = uid.ToString().ToUpper();
            await this._hubContext.SendNotification(method, userId, noti);
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] NotificationCreate itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_Notification>(itemModel);
            if (item == null)
                throw new AppException("Item không tồn tại");

            await Validate(item);
            success = await this.domainService.CreateAsync(item);
            if (!success)
                throw new Exception("Lỗi trong quá trình xử lý");

            SendNotification(item.userId.Value);

            appDomainResult.success = success;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = "Thành công";

            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [NonAction]
        [HttpGet]
        [Description("Lấy danh sách")]
        public override async Task<AppDomainResult> Get([FromQuery] NotificationSearch baseSearch)
        {
            return new AppDomainResult();
        }

        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("Lấy danh sách")]
        public virtual async Task<AppDomainResult> GetAll([FromQuery] NotificationSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            PagedList<tbl_Notification> pagedData = await this.domainService.GetPagedListData(baseSearch);
            return new AppDomainResult(pagedData);
        }

        //[HttpGet("")]
        //[Description("Lấy danh sách top 5 noti chưa xem")]
        //public async Task<AppDomainResult> GetIsNotSeenNotification(Guid id,[FromQuery] int? count)
        //{
        //    if (!ModelState.IsValid)
        //        throw new AppException(ModelState.GetErrorMessage());
        //    var notificationList = await _notificationService.GetNotificationIsNotSeen(id, count??5);
        //    return new AppDomainResult(notificationList);
        //}

        /// <summary>
        /// Xem tin nhắn
        /// </summary>
        /// <param name="itemModel">Id của Noti hoặc User tùy vào Type, Type : 1 Noti; 2 User</param>
        /// <returns></returns>
        [HttpPut]
        [AppAuthorize]
        [Description("Xem tin nhắn")]
        public override async Task<AppDomainResult> UpdateItem([FromBody] NotificationUpdate itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_Notification>(itemModel);
            if (item == null)
                throw new KeyNotFoundException("Item không tồn tại");
            if (itemModel.type == 1)
            {
                var currentItem = await domainService.GetByIdAsync(item.id);
                if (item == null)
                    throw new KeyNotFoundException("Thông báo không tồn tại");
                //set is seen = true
                currentItem.isSeen = true;
                success = await this.domainService.UpdateAsync(currentItem);
                if (!success)
                    throw new Exception("Lỗi trong quá trình xử lý");
                SendNotification(currentItem.userId.Value);
            }
            else
            {
                 success = await _notificationService.UpdateNotificationToSeenOfUser(item.id);
                if (!success)
                    throw new Exception("Lỗi trong quá trình xử lý");

            }
            appDomainResult.success = success;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = "Thành công";

            return appDomainResult;
        }


        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("get-is-notification-list")]
        [Description("Lấy danh sách")]
        public async Task<AppDomainResult> GetNotificationList([FromQuery] NotificationSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            PagedList<tbl_Notification> pagedData = await _notificationService.GetPagedListData(baseSearch);
            return new AppDomainResult(pagedData);
        }


        /// <summary>
        /// Xem tin nhắn
        /// </summary>
        /// <param name="itemModel">Id của Noti hoặc User tùy vào Type, Type : 1 Noti; 2 User</param>
        /// <returns></returns>
        [HttpPut("update-is-seen")]
        [Description("Xem tin nhắn")]
        public async Task<AppDomainResult> UpdateIsSeen([FromBody] NotificationUpdate itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_Notification>(itemModel);
            if (item == null)
                throw new KeyNotFoundException("Item không tồn tại");
            if (itemModel.type == 1)
            {
                var currentItem = await domainService.GetByIdAsync(item.id);
                if (item == null)
                    throw new KeyNotFoundException("Thông báo không tồn tại");
                //set is seen = true
                currentItem.isSeen = true;
                success = await this.domainService.UpdateAsync(currentItem);
                if (!success)
                    throw new Exception("Lỗi trong quá trình xử lý");
                //SendNotification(currentItem.userId.Value);
            }
            else
            {
                var notificationList = await _notificationService.UpdateNotificationToSeenOfUser(item.id);
            }
            appDomainResult.success = success;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = "Thành công";

            return appDomainResult;
        }
    }
}
