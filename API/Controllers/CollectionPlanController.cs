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
using Azure.Core;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Kế hoạch thu")]
    [Authorize]
    public class CollectionPlanController : BaseController<tbl_CollectionPlan, CollectionPlanCreate, CollectionPlanUpdate, CollectionPlanSearch>
    {
        private readonly ICollectionPlanService collectionPlanService;
        public CollectionPlanController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_CollectionPlan, CollectionPlanCreate, CollectionPlanUpdate, CollectionPlanSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<ICollectionPlanService>();
            this.collectionPlanService = serviceProvider.GetRequiredService<ICollectionPlanService>();
        }

        /// <summary>
        /// Tổng thu nhập 
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        [HttpGet("total-income")]
        [AppAuthorize]
        [Description("Tổng thu nhập theo tháng/năm/...")]
        public async Task<AppDomainResult> TotalIncome([FromQuery] CollectionPlanReport baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());

            var data = await this.collectionPlanService.TotalIncome(baseSearch);

            return new AppDomainResult(data);
        }

        /// <summary>
        /// Trung bình tiền thu mỗi học sinh
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        [HttpGet("average-per-student")]
        [AppAuthorize]
        public async Task<AppDomainResult> AvgMoneyPerStudent([FromQuery] AverageMoneyPerStudent baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());

            var data = await this.collectionPlanService.AvgMoneyPerStudent(baseSearch);

            return new AppDomainResult(data);
        }

        /// <summary>
        /// Thống kê theo mỗi đợt thu của 1 kế hoạch thu
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        [HttpGet("collection-plan")]
        [AppAuthorize]
        public async Task<AppDomainResult> CollectionSessionReport([FromQuery] CollectionSessionReport baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());

            var data = await this.collectionPlanService.CollectionSessionReport(baseSearch);

            return new AppDomainResult(data);
        }

        /// <summary>
        /// Thống kê theo các khoảng thu 
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        [HttpGet("fee")]
        [AppAuthorize]
        public async Task<AppDomainResult> ReportByFee([FromQuery] ReportByFee baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());

            var data = await this.collectionPlanService.ReportByFee(baseSearch);

            return new AppDomainResult(data);
        }

        /// <summary>
        /// Danh sách các đợt thu còn nợ
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        [HttpGet("debt")]
        [AppAuthorize]
        public async Task<AppDomainResult> CollectionSessionDebt([FromQuery] CollectionSessionDebtSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());

            var data = await this.collectionPlanService.CollectionSessionDebt(baseSearch);

            return new AppDomainResult(data);
        }

        /// <summary>
        /// Cập nhật trạng thái đợt thu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("update-status")]
        [AppAuthorize]
        public async Task<AppDomainResult> UpdateStatus([FromBody] CollectionPlanUpdateStatus request)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());

            var data = await this.collectionPlanService.UpdateStatusAndGenerate(request);

            return new AppDomainResult(data);
        }
    }
}
