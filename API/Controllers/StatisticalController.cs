using BaseAPI.Controllers;
using Entities.Search;
using Entities;
using Extensions;
using Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Request.RequestCreate;
using Request.RequestUpdate;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Utilities;
using Microsoft.Extensions.DependencyInjection;
using Entities.Model.Statistical;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Báo cáo thống kê")]
    [Authorize]
    public class StatisticalController : ControllerBase
    {
        private readonly IStatisticalService statisticalService;
        public StatisticalController(IServiceProvider serviceProvider)
        {
            this.statisticalService = serviceProvider.GetRequiredService<IStatisticalService>();
        }

        /// <summary>
        /// Báo cáo số liệu tổng quan
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("FinanceOverview")]
        [AppAuthorize]
        [Description("Báo cáo số liệu tổng quan")]
        public async Task<List<StatisticalModel>> FinanceOverview([FromQuery] StatisticalSearch baseSearch)
        {
            var data = await this.statisticalService.FinanceOverview(baseSearch);
            return data;
        }

        /// <summary>
        /// Báo cáo doanh thu 12 tháng
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("Revenue12Month")]
        [AppAuthorize]
        [Description("Báo cáo doanh thu 12 tháng")]
        public async Task<List<Statistical12MonthModel>> Revenue12Month([FromQuery] StatisticalSearch baseSearch)
        {
            var data = await this.statisticalService.Revenue12Month(baseSearch);
            return data;
        }

        /// <summary>
        /// Tỷ lệ khoản thu
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("RateTutionConfig")]
        [AppAuthorize]
        [Description("Tỷ lệ khoản thu")]
        public async Task<List<StatisticalModel>> RateTutionConfig([FromQuery] StatisticalSearch baseSearch)
        {
            var data = await this.statisticalService.RateTutionConfig(baseSearch);
            return data;
        }

        /// <summary>
        /// Học viên còn nợ
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("ReportBillInDebt")]
        [AppAuthorize]
        [Description("Học viên còn nợ")]
        public async Task<AppDomainResult> ReportBillInDebt([FromQuery] StatisticalSearch baseSearch)
        {
            var data = await this.statisticalService.ReportBillInDebt(baseSearch);
            return new AppDomainResult(data);
        }
    }
}
