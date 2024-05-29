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
using static Utilities.CoreContants;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Hangfire;
using System.Net.WebSockets;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Màn hình trả lời phản hồi")]
    [Authorize]
    public class FeedbackReplyController : BaseController<tbl_FeedbackReply, FeedbackReplyCreate, FeedbackReplyUpdate, FeedbackReplySearch>
    {
        private readonly IFeedbackReplyService feedbackReplyService; 
        public FeedbackReplyController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_FeedbackReply, FeedbackReplyCreate, FeedbackReplyUpdate, FeedbackReplySearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.feedbackReplyService = serviceProvider.GetRequiredService<IFeedbackReplyService>();
            this.domainService = serviceProvider.GetRequiredService<IFeedbackReplyService>();
        }
       
        /// <summary>
        /// Đánh giá kết quả phản hồi
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("vote")]
        [AppAuthorize]
        [Description("Đánh giá kết quả phản hồi")]
        public virtual async Task<AppDomainResult> VoteFeedback([FromBody] FeedbackVote itemModel)
        {
            await this.feedbackReplyService.Vote(itemModel);
            return new AppDomainResult();
        }

        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] FeedbackReplyCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_FeedbackReply>(itemModel);
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            var data = await this.feedbackReplyService.AddItemWithResponse(item);
            return new AppDomainResult(data);
        }

        //[NonAction]
        //public override Task<AppDomainResult> UpdateItem([FromBody] DomainUpdate itemModel) => base.UpdateItem(itemModel);
        [NonAction]
        public override Task<AppDomainResult> GetById([FromBody] Guid id) => base.GetById(id);
        [NonAction]
        public override Task<AppDomainResult> Get([FromBody] FeedbackReplySearch itemModel) => base.Get(itemModel);
    }
}
