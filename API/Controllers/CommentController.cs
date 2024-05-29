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

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Nhận xét hằng ngày")]
    [Authorize]
    public class CommentController : BaseController<tbl_Comment, CommentCreate, CommentUpdate, CommentSearch>
    {
        private readonly ICommentService commentService;
        public CommentController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Comment, CommentCreate, CommentUpdate, CommentSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<ICommentService>();
            this.commentService = serviceProvider.GetRequiredService<ICommentService>();
        }
        [NonAction]
        public override Task<AppDomainResult> Get([FromQuery] CommentSearch baseSearch)
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
        public override Task<AppDomainResult> AddItem([FromBody] CommentCreate itemModel)
        {
            return base.AddItem(itemModel);
        }
        [NonAction]
        public override Task<AppDomainResult> UpdateItem([FromBody] CommentUpdate itemModel)
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
        public async Task<AppDomainResult> GetOrGenerateComment([FromQuery] CommentSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.commentService.GetOrGenerateComment(baseSearch);
            return new AppDomainResult(data);
        }
        /// <summary>
        /// Cập nhật nhận xét hàng ngày 
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateComment")]
        [AppAuthorize]
        [Description("Cập nhật nhận xét hàng ngày")]
        public async Task<AppDomainResult> UpdateComment([FromBody] List<CommentUpdate> itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            bool success = true;
            foreach (var item in itemModel)
            {
                var data = mapper.Map<tbl_Comment>(item);
                success = await this.commentService.UpdateAsync(data);
                if (!success)
                    throw new AppException("Lỗi trong quá trình xử lý!");
            }
            return new AppDomainResult()
            {
                resultCode = (int)HttpStatusCode.OK,
                resultMessage = "Cập nhật thành công!",
                success = success
            };
        }

        [HttpGet("by-student")]
        [AppAuthorize]
        [Description("Get by student")]
        public async Task<AppDomainResult> GetByStudent([FromQuery] CommentSearch search)
        {
            var result = new tbl_Comment();
            var data = await this.commentService.GetOrGenerateComment(search);
            if (data.items != null && data.items.Count > 0)
                result = data.items.FirstOrDefault();
            return new AppDomainResult(result);
        }


        [HttpPost("notify")]
        [AppAuthorize]
        [Description("Thông báo cho tất cả phụ huynh của các bé trong lớp")]
        public async Task<AppDomainResult> Notify(CommentNotificationRequest request)
        {
            await commentService.SendNotification(request);
            return new AppDomainResult();
        }
    }
}
