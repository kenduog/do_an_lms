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
using Entities.AuthEntities;
using AppDbContext;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Tham gia nhóm bảng tin")]
    [Authorize]
    public class UserJoinGroupNewsController : ControllerBase
    {
        protected IUserJoinGroupNewsService domainService;
        protected IUserJoinGroupNewsService userJoinGroupNewsService;
        protected ISendNotificationService sendNotificationService;
        protected IUserService userService;
        protected IGroupNewsService groupNewsService;

        protected IWebHostEnvironment env;
        public UserJoinGroupNewsController(
            IServiceProvider serviceProvider
            , IWebHostEnvironment env)
        {
            this.domainService = serviceProvider.GetRequiredService<IUserJoinGroupNewsService>();
            this.userJoinGroupNewsService = serviceProvider.GetRequiredService<IUserJoinGroupNewsService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.sendNotificationService = serviceProvider.GetRequiredService<ISendNotificationService>();
            this.groupNewsService = serviceProvider.GetRequiredService<IGroupNewsService>();
        }
        /// <summary>
        /// Tham gia
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Tham gia")]
        public async Task<AppDomainResult> JoinGroupNews([FromBody] UserJoinGroupNewsCreate itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            await this.userJoinGroupNewsService.Validate(itemModel.groupNewsId);
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            List<tbl_Users> receiverList = new List<tbl_Users>();
            var groupNews = await groupNewsService.GetByIdAsync(itemModel.groupNewsId);
            List<IDictionary<string, string>> notiParamList = new List<IDictionary<string, string>>();
            var data = await this.domainService.GetSingleAsync(x => x.userId == itemModel.userId && x.groupNewsId == itemModel.groupNewsId);
            if (data != null)
            {
                data.deleted = false;
                data.userType = 2;
                success = await this.userJoinGroupNewsService.Update(data);
            }
            else
            {
                tbl_UserJoinGroupNews item = new tbl_UserJoinGroupNews()
                {
                    groupNewsId = itemModel.groupNewsId,
                    userId = itemModel.userId,
                    userType = 2,
                };
                if (item == null)
                    throw new AppException(MessageContants.nf_item);
                success = await this.domainService.CreateAsync(item);
            }
            if (!success)
                throw new AppException(MessageContants.err);
            var user = await userService.GetByIdAsync(itemModel.userId);
            if (user != null)
            {
                receiverList.Add(user);
                IDictionary<string, string> notiParam = new Dictionary<string, string>();
                notiParam.Add("[GroupNewsName]", groupNews.name);
                notiParamList.Add(notiParam);
                string linkQuery = "group=" + sendNotificationService.EncodingParam(groupNews.id.ToString());
                string subLink = string.Empty;
                sendNotificationService.SendNotification(Guid.Parse("ded2f0e3-ba3e-4969-6fe3-08dc17fc7766"), receiverList, notiParamList, null, linkQuery, null, LookupConstant.ScreenCode_GroupNews, subLink);
            }
            appDomainResult.success = success;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = MessageContants.success;

            return appDomainResult;
        }
        /// <summary>
        /// Ra khỏi nhóm
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpDelete]
        [AppAuthorize]
        [Description("Ra khỏi nhóm")]
        public async Task<AppDomainResult> OutGroupNews([FromQuery] UserJoinGroupNewsCreate itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var user = LoginContext.Instance.CurrentUser;
            if (user.userId != itemModel.userId)
                await this.userJoinGroupNewsService.Validate(itemModel.groupNewsId);
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.domainService.GetSingleAsync(x => x.userId == itemModel.userId && x.groupNewsId == itemModel.groupNewsId);
            if (data == null)
                throw new AppException(MessageContants.nf_userInGroupNews);
            tbl_UserJoinGroupNews item = new tbl_UserJoinGroupNews()
            {
                id = data.id,
                groupNewsId = itemModel.groupNewsId,
                userId = itemModel.userId,
                deleted = true,
            };
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            success = await this.domainService.UpdateAsync(item);
            if (!success)
                throw new AppException(MessageContants.err);

            appDomainResult.success = success;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = MessageContants.success;

            return appDomainResult;
        }
        /// <summary>
        /// Nhiều người ra khỏi nhóm
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("multiple")]
        [AppAuthorize]
        [Description("Nhiều người ra khỏi nhóm")]
        public async Task<AppDomainResult> JoinGroupMultipleNews([FromQuery] UserJoinGroupNewsMultipleCreate itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            await this.userJoinGroupNewsService.Validate(itemModel.groupNewsId);
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var arrUser = itemModel.userIds.Split(',');

            foreach (var u in arrUser)
            {
                var userDelete = Guid.Parse(u);
                var data = await this.domainService.GetSingleAsync(x => x.userId == userDelete && x.groupNewsId == itemModel.groupNewsId);
                if (data == null)
                    throw new AppException(MessageContants.nf_userInGroupNews);
                if (data.userType == 1)
                    throw new AppException(MessageContants.can_not_delete_owner_group_news);
                tbl_UserJoinGroupNews item = new tbl_UserJoinGroupNews()
                {
                    id = data.id,
                    groupNewsId = itemModel.groupNewsId,
                    userId = userDelete,
                    deleted = true,
                };
                if (item == null)
                    throw new AppException(MessageContants.nf_item);
                success = await this.domainService.UpdateAsync(item);
                if (!success)
                    throw new AppException(MessageContants.err);

            }

            appDomainResult.success = success;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = MessageContants.success;

            return appDomainResult;
        }
        /// <summary>
        /// Nhiều người tham gia
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("multiple")]
        [AppAuthorize]
        [Description("Nhiều người ra tham gia")]
        public async Task<AppDomainResult> OutGroupMultipleNews([FromBody] UserJoinGroupNewsMultipleCreate itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            await this.userJoinGroupNewsService.Validate(itemModel.groupNewsId);
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var arrUser = itemModel.userIds.Split(',');
            List<tbl_Users> receiverList = new List<tbl_Users>();
            var groupNews = await groupNewsService.GetByIdAsync(itemModel.groupNewsId) ?? throw new AppException(MessageContants.nf_groupNews); ;
            List<IDictionary<string, string>> notiParamList = new List<IDictionary<string, string>>();
            foreach (var u in arrUser)
            {
                var userCreate = Guid.Parse(u);
                var data = await this.domainService.GetSingleAsync(x => x.userId == userCreate && x.groupNewsId == itemModel.groupNewsId);
                if (data != null)
                {
                    data.deleted = false;
                    data.userType = 2;
                    success = await this.userJoinGroupNewsService.Update(data);
                }
                else
                {
                    tbl_UserJoinGroupNews item = new tbl_UserJoinGroupNews()
                    {
                        groupNewsId = itemModel.groupNewsId,
                        userId = userCreate,
                        userType = 2,
                    };
                    if (item == null)
                        throw new AppException(MessageContants.nf_item);
                    success = await this.domainService.CreateAsync(item);
                }
                if (!success)
                    throw new AppException(MessageContants.err);
                var user = await userService.GetByIdAsync(userCreate);
                if (user != null)
                {
                    receiverList.Add(user);
                    IDictionary<string, string> notiParam = new Dictionary<string, string>();
                    notiParam.Add("[GroupNewsName]", groupNews.name);
                    notiParamList.Add(notiParam);
                }
            }
            string linkQuery = "group=" + sendNotificationService.EncodingParam(groupNews.id.ToString());
            string subLink = string.Empty;
            sendNotificationService.SendNotification(Guid.Parse("ded2f0e3-ba3e-4969-6fe3-08dc17fc7766"), receiverList, notiParamList, null, linkQuery, null, LookupConstant.ScreenCode_GroupNews, subLink);
            appDomainResult.success = success;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = MessageContants.success;
            return appDomainResult;
        }
    }
}
