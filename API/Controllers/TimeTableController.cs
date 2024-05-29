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
    public class TimeTableController : BaseController<tbl_TimeTable, TimeTableCreate, TimeTableUpdate, TimeTableSearch>
    {
        private readonly ITimeTableService timeTableService;
        public TimeTableController(IServiceProvider serviceProvider
            , ILogger<BaseController<tbl_TimeTable, TimeTableCreate, TimeTableUpdate, TimeTableSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<ITimeTableService>();
            this.timeTableService = serviceProvider.GetRequiredService<ITimeTableService>();
        }


        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] TimeTableCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data= await this.timeTableService.CreateTimeTable(itemModel);
            return new AppDomainResult(data);
        }

        /// <summary>
        /// Lấy thông tin chi tiết tkb
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AppAuthorize]
        [Description("Lấy thông tin")]
        public override async Task<AppDomainResult> GetById(Guid id)
        {
            var item = await this.timeTableService.GenerateTimetable(id);
            return new AppDomainResult(item);
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public override async Task<AppDomainResult> Get([FromQuery] TimeTableSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            PagedList<tbl_TimeTable> pagedData = await this.domainService.GetPagedListData(baseSearch);
            return new AppDomainResult(pagedData);
        }

       
        #region comment code a Hung
        //[HttpPut]
        //[AppAuthorize]
        //[Description("Chỉnh sửa")]
        //public override async Task<AppDomainResult> UpdateItem([FromBody] TimeTableUpdate itemModel)
        //{
        //    using (var tran = await appDbContext.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            var entity = mapper.Map<tbl_TimeTable>(itemModel);
        //            if (entity == null)
        //                throw new AppException(MessageContants.nf_item);
        //            await this.timeTableService.ValidateForUpdate(entity);
        //            await this.domainService.UpdateItem(entity);

        //            return new AppDomainResult();
        //        }
        //        catch (AppException e)
        //        {
        //            await tran.RollbackAsync();
        //            throw new AppException(e.Message + " || " + e.InnerException);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Thêm mới nhiều item
        ///// </summary>
        ///// <param name="itemModel"></param>
        ///// <returns></returns>
        //[HttpPost("Multiple")]
        //[AppAuthorize]
        //[Description("Thêm mới nhiều items")]
        //public async Task<AppDomainResult> AddItems([FromBody] List<TimeTableCreate> itemModel)
        //{
        //    using (var tran = await appDbContext.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            foreach (var item in itemModel)
        //            {
        //                var entity = mapper.Map<tbl_TimeTable>(item);
        //                if (entity == null)
        //                    throw new AppException(MessageContants.nf_item);
        //                await Validate(entity);
        //                await this.domainService.AddItem(entity);
        //            }

        //            return new AppDomainResult();
        //        }
        //        catch (AppException e)
        //        {
        //            await tran.RollbackAsync();
        //            throw new AppException(e.Message + " || " + e.InnerException);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Hàm này dùng để kiểm tra xem lịch có phù hợp với giáo viên, phòng học khi tạo hay không
        ///// </summary>
        ///// <param name="itemModel"></param>
        ///// <returns></returns>
        ///// <exception cref="AppException"></exception>
        //[HttpPost("CheckForInsert")]
        //[AppAuthorize]
        //[Description("Kiểm tra lịch có phù hợp không khi thêm mới")]
        //public async Task<AppDomainResult> CheckForInsert([FromBody] TimeTableCreate itemModel)
        //{
        //    try
        //    {
        //        var entity = mapper.Map<tbl_TimeTable>(itemModel);
        //        if (entity == null)
        //            throw new AppException(MessageContants.nf_item);
        //        await Validate(entity);
        //        return new AppDomainResult(entity);
        //    }
        //    catch (AppException e)
        //    {
        //        throw new AppException(e.Message + " || " + e.InnerException);
        //    }
        //}
        ///// <summary>
        ///// Hàm này dùng để kiểm tra xem lịch có phù hợp với giáo viên, phòng học khi cập nhật hay không
        ///// Phải tách riêng không dùng chung là vì phải loại trừ không check buổi được update 
        ///// </summary>
        ///// <param name="itemModel"></param>
        ///// <returns></returns>
        ///// <exception cref="AppException"></exception>
        //[HttpPost("CheckForUpdate")]
        //[AppAuthorize]
        //[Description("Kiểm tra lịch có phù hợp không khi cập nhật")]
        //public async Task<AppDomainResult> CheckForUpdate([FromBody] TimeTableUpdate itemModel)
        //{
        //    try
        //    {
        //        var entity = mapper.Map<tbl_TimeTable>(itemModel);
        //        if (entity == null)
        //            throw new AppException(MessageContants.nf_item);
        //        await this.timeTableService.ValidateForUpdate(entity);

        //        return new AppDomainResult(entity);
        //    }
        //    catch (AppException e)
        //    {
        //        throw new AppException(e.Message + " || " + e.InnerException);
        //    }
        //}

        ///// <summary>
        ///// Tự tạo thời khóa biểu theo mẫu
        ///// </summary>
        ///// <param name="itemModel"></param>
        ///// <returns></returns>
        ///// <exception cref="AppException"></exception>
        //[HttpPost("Generate")]
        //[AppAuthorize]
        //[Description("Tự động tạo thời khóa biểu theo mẫu")]
        //public async Task<AppDomainResult> CheckForUpdate([FromBody] GenerateTimeTableCreate itemModel)
        //{
        //    try
        //    {
        //        var data = await this.timeTableService.GenerateTimeTable(itemModel);
        //        return new AppDomainResult(data);
        //    }
        //    catch (AppException e)
        //    {
        //        throw new AppException(e.Message + " || " + e.InnerException);
        //    }
        //}

        #endregion
    }
}
