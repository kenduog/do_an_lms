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
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Tài liệu bảng tin")]
    [Authorize]
    public class DocumentNewsController : NecessaryController
    {
        protected IDocumentNewsService domainService;
        protected readonly IMapper mapper;
        public DocumentNewsController(INecessaryService necessaryService, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider) : base(necessaryService, env, httpContextAccessor)
        {
            this.domainService = serviceProvider.GetRequiredService<IDocumentNewsService>();
        }

        /// <summary>
        /// Tải lên
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Tải lên")]
        public async Task<AppDomainResult> UploadSingle([FromBody] DocumentNewsCreate itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = new tbl_DocumentNews();
            item.newsId = itemModel.newsId;
            item.commentId = itemModel.commentId;
            //item.typeCode = GetTypeDocument(itemModel.link);
            item.link = await this.UploadFile(itemModel.link, "DocumentNews");
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            success = await this.domainService.CreateAsync(item);
            if (!success)
                throw new AppException(MessageContants.err);

            appDomainResult.success = success;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = MessageContants.success;
            return appDomainResult;
        }
        #region NoAction
        [NonAction]
        public override async Task<AppDomainResult> UploadImage(IFormFile file)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            appDomainResult.success = true;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = MessageContants.success;
            return appDomainResult;
        }
        [NonAction]
        public override async Task<AppDomainResult> UploadFiles(List<IFormFile> files)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            appDomainResult.success = true;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = MessageContants.success;
            return appDomainResult;
        }
        [NonAction]
        public int GetTypeDocument(IFormFile file)
        {
            int result = 3;
            if (file.ContentType.IndexOf("image") != -1)
                result = 0; // ảnh
            if (file.ContentType.IndexOf("video") != -1)
                result = 1; // video
            if (file.ContentType.IndexOf("audio") != -1)
                result = 2; // audio
            return result;
        }
        #endregion
    }
}
