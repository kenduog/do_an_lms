using BaseAPI.Controllers;
using Entities;
using Entities.Search;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Request.RequestCreate;
using Request.RequestUpdate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Utilities;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Thời khóa biểu")]
    [Authorize]
    public class TimeTableDetailController : BaseController<tbl_TimeTableDetail, TimeTableDetailCreate, TimeTableDetailUpdate, TimeTableDetailSearch>
    {
        public TimeTableDetailController(IServiceProvider serviceProvider
            , IAppDbContext appDbContext
            , ILogger<BaseController<tbl_TimeTableDetail, TimeTableDetailCreate, TimeTableDetailUpdate, TimeTableDetailSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<ITimeTableDetailService>();
        }
    }
}
