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
    [Description("Bình luận bài đăng")]
    [Authorize]
    public class CommentInNewsController : BaseController<tbl_CommentInNews, CommentInNewsCreate, CommentInNewsUpdate, CommentInNewsSearch>
    {
        private readonly INewsService newsService;
        public CommentInNewsController(IServiceProvider serviceProvider
            , ILogger<BaseController<tbl_CommentInNews, CommentInNewsCreate, CommentInNewsUpdate, CommentInNewsSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<ICommentInNewsService>();
            this.newsService = serviceProvider.GetRequiredService<INewsService>();
        }
        /// <summary>
        /// Thêm bình luận
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm bình luận")]
        public override async Task<AppDomainResult> AddItem([FromBody] CommentInNewsCreate itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var hasNews = await newsService.AnyAsync(x => x.id == itemModel.newsId);
            if (!hasNews)
                throw new AppException("Không tìm thấy bảng tin");
            var userId = LoginContext.Instance.CurrentUser.userId;

            tbl_CommentInNews item = new tbl_CommentInNews()
            {
                newsId = itemModel.newsId,
                content = itemModel.content,
                userId = userId,
                replyCommentId = itemModel.replyCommentId,
            };
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
        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut]
        [AppAuthorize]
        [Description("Chỉnh sửa")]
        [NonAction]
        public override async Task<AppDomainResult> UpdateItem([FromBody] CommentInNewsUpdate itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_CommentInNews>(itemModel);

            await Validate(item);
            if (item == null)
                throw new KeyNotFoundException(MessageContants.nf_item);
            success = await this.domainService.UpdateAsync(item);
            if (!success)
                throw new Exception(MessageContants.err);

            appDomainResult.success = success;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = MessageContants.success;

            return appDomainResult;
        }
        /// <summary>
        /// Xóa item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AppAuthorize]
        [Description("Xoá")]
        public override async Task<AppDomainResult> DeleteItem(Guid id)
        {
            var commentChild = await this.domainService.GetAsync(x => x.replyCommentId == id);
            if (commentChild.Count > 0)
                foreach (var comment in commentChild)
                    await this.domainService.DeleteItem(comment.id);
            await this.domainService.DeleteItem(id);
            return new AppDomainResult();
        }
    }
}
