using Entities;
using Extensions;
using Interface.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Entities.Search;
using Request.RequestCreate;
using Request.RequestUpdate;
using BaseAPI.Controllers;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Cấu hình học phí")]
    [Authorize]
    public class ItemController : BaseController<tbl_Item, ItemCreate, ItemUpdate, ItemSearch>
    {
        public ItemController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Item, ItemCreate, ItemUpdate, ItemSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<IItemService>();
        }
    }
}
