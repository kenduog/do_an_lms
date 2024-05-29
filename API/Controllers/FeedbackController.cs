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
using AppDbContext;
using Service.Services;
using System.Text;
using Interface.DbContext;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Màn hình phản hồi")]
    [Authorize]
    public class FeedbackController : BaseController<tbl_Feedback, FeedbackCreate, DomainUpdate, FeedbackSearch>
    {
        private readonly IFeedbackService feedbackService;
        public FeedbackController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Feedback, FeedbackCreate, DomainUpdate, FeedbackSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IFeedbackService>();
            this.feedbackService = serviceProvider.GetRequiredService<IFeedbackService>();
        }

        [NonAction]
        public override async Task<AppDomainResult> UpdateItem([FromBody] DomainUpdate itemModel) => await base.UpdateItem(itemModel);

        /// <summary>
        /// Lấy danh sách item phân trang chia tab theo status
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public override async Task<AppDomainResult> Get([FromQuery] FeedbackSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var pagedData = await this.feedbackService.GetPagedStatus(baseSearch);
            return new AppDomainResult(pagedData);
        }

        /// <summary>
        /// Cập nhật trạng thái đã xong
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("done/{id}")]
        [AppAuthorize]
        [Description("Cập nhật trạng thái đã xong")]
        public virtual async Task<AppDomainResult> DoneFeedback(Guid id)
        {
            await this.feedbackService.Done(id);
            return new AppDomainResult();
        }
    }
}
