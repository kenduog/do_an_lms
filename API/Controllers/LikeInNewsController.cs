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

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Lượt thích")]
    [Authorize]
    public class LikeInNewsController : ControllerBase
    {
        protected ILikeInNewsService domainService;
        protected IWebHostEnvironment env;
        public LikeInNewsController(
            IServiceProvider serviceProvider
            , IWebHostEnvironment env)
        {
            this.domainService = serviceProvider.GetRequiredService<ILikeInNewsService>();
        }
        /// <summary>
        /// Thích & bỏ thích
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Like & Unlike")]
        public async Task<AppDomainResult> AddItem([FromBody] LikeInNewsCreate itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var accLog = LoginContext.Instance.CurrentUser;
            var data = await this.domainService.GetSingleAsync(x => x.userId == accLog.userId && x.newsId == itemModel.newsId);
            tbl_LikeInNews item = new tbl_LikeInNews()
            {
                newsId = itemModel.newsId,
                userId = accLog.userId,
                isLike =  true,
            };
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            if (data == null)
            {
                success = await this.domainService.CreateAsync(item);
            }
            else
            {
                item.id = data.id; 
                item.isLike = !(data.isLike);
                success = await this.domainService.UpdateAsync(item);
            }
            if (!success)
                throw new AppException(MessageContants.err);

            appDomainResult.success = success;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = MessageContants.success;

            return appDomainResult;
        }
    }
}
