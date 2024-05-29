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
    [Description("Món ăn")]
    [Authorize]
    public class FoodItemController : BaseController<tbl_FoodItem, FoodItemCreate, FoodItemUpdate, FoodItemSearch>
    {
        private readonly IFoodItemService foodItemService;
        public FoodItemController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_FoodItem, FoodItemCreate, FoodItemUpdate, FoodItemSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.foodItemService = serviceProvider.GetRequiredService<IFoodItemService>();
            this.domainService = serviceProvider.GetRequiredService<IFoodItemService>();
        }

        /// <summary>
        /// Lấy danh sách thành phần của những món ăn 
        /// </summary>
        /// <returns></returns>
        [HttpGet("food-item-menu")]
        [AppAuthorize]
        [Description("Lấy danh sách thành phần của những món ăn ")]
        public async Task<AppDomainResult> GetFoodItemMenu([FromQuery] FoodItemMenuRequest search)
        {
            var data = await this.foodItemService.GetFoodItemMenu(search);
            return new AppDomainResult(data);
        }
    }
}
