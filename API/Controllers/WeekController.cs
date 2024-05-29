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
using Azure.Core;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Cấu hình tuần học")]
    [Authorize]
    public class WeekController : BaseController<tbl_Week, WeekCreate, WeekUpdate, WeekSearch>
    {
        private readonly IWeekService weekService;
        private readonly ISemesterService semesterService;
        private readonly ISchoolYearService schoolYearService;
        public WeekController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Week, WeekCreate, WeekUpdate, WeekSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<IWeekService>();
            this.weekService = serviceProvider.GetRequiredService<IWeekService>();
            this.semesterService = serviceProvider.GetRequiredService<ISemesterService>();
            this.schoolYearService = serviceProvider.GetRequiredService<ISchoolYearService>();
        }
        [NonAction]
        public override async Task Validate(tbl_Week model)
        {
            // create
            if(model.id == Guid.Empty)
            {
                if (model.schoolYearId.HasValue)
                {
                    var item = await schoolYearService
                        .AnyAsync(x => x.id == model.schoolYearId && x.deleted == false);
                    if (!item)
                        throw new AppException(MessageContants.nf_schoolYear);
                }
                if (model.semesterId.HasValue)
                {
                    var item = await semesterService.AnyAsync(x => x.schoolYearId == model.schoolYearId && x.id == model.semesterId && x.deleted == false);
                    if (!item)
                        throw new AppException(MessageContants.nf_semester);
                }
                var week = await domainService.GetAsync(x => x.schoolYearId == model.schoolYearId && x.semesterId == model.semesterId && x.deleted == false);
                if (model.sTime.HasValue && model.eTime.HasValue)
                {
                    if (model.sTime > model.eTime)
                        throw new AppException("Thời gian bắt đầu không được phép nhỏ hơn thời gian kết thúc!");
                    if (week != null)
                    {
                        int checkSTime = week.Count(x => x.sTime <= model.sTime && x.eTime >= model.sTime);
                        int checkETime = week.Count(x => x.sTime <= model.eTime && x.eTime >= model.eTime);
                        if (checkSTime > 0 || checkETime > 0)
                            throw new AppException("Thời gian đã trùng vào tuần học khác vui lòng chọn lại!");
                    }
                    var semester = await this.semesterService.GetByIdAsync(model.semesterId.Value);
                    bool bSTime = semester.sTime <= model.sTime && semester.eTime >= model.sTime;
                    bool bETime = semester.sTime <= model.eTime && semester.eTime >= model.eTime;
                    if (!bSTime || !bETime)
                        throw new AppException("Thời gian vượt quá thời gian bắt đầu và kết thúc của học kỳ!");
                }
            }
            // update
            else
            {
                var weekUpdate = await domainService.GetByIdAsync(model.id);
                var week = await domainService.GetAsync(x => x.schoolYearId == weekUpdate.schoolYearId && x.semesterId == weekUpdate.semesterId && x.deleted == false && x.id != model.id);
                if (model.sTime.HasValue && model.eTime.HasValue)
                {
                    if (model.sTime > model.eTime)
                        throw new AppException("Thời gian bắt đầu không được phép nhỏ hơn thời gian kết thúc!");
                    if (week != null)
                    {
                        int checkSTime = week.Count(x => x.sTime <= model.sTime && x.eTime >= model.sTime);
                        int checkETime = week.Count(x => x.sTime <= model.eTime && x.eTime >= model.eTime);
                        if (checkSTime > 0 || checkETime > 0)
                            throw new AppException("Thời gian đã trùng vào tuần học khác vui lòng chọn lại!");
                    }
                    var semester = await this.semesterService.GetByIdAsync(weekUpdate.semesterId.Value);
                    bool bSTime = semester.sTime <= model.sTime && semester.eTime >= model.sTime;
                    bool bETime = semester.sTime <= model.eTime && semester.eTime >= model.eTime;
                    if (!bSTime || !bETime)
                        throw new AppException("Thời gian vượt quá thời gian bắt đầu và kết thúc của học kỳ!");
                }
            }
        }
        /// <summary>
        /// Generate Week 
        /// </summary>
        /// <param name="generate"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GenerateWeek")]
        [AppAuthorize]
        [Description("Generate Week")]
        public async Task<AppDomainResult> GenerateWeek([FromBody] GenerateWeekCreate generate)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var checkSemester = await this.semesterService.CountAsync(x => x.id == generate.semesterId && x.schoolYearId == generate.schoolYearId && x.deleted == false);
            if (checkSemester == 0)
                throw new AppException("Học kỳ không tồn tại!");
            var checkData = await this.domainService.CountAsync(x => x.semesterId == generate.semesterId && x.schoolYearId == generate.schoolYearId && x.deleted == false);
            if (checkData > 0)
                throw new AppException("Tuần đã tồn tại!");
            var week = await domainService.GetAsync(x => x.schoolYearId == generate.schoolYearId && x.deleted == false);
            int numberWeek = 0;
            if(week.Count > 0)
                numberWeek = week.Max(x=>x.weekNumber).Value;
            var data = await this.weekService.GenerateWeek(generate, numberWeek);
            return data;
        }
        /// <summary>
        /// Lấy tuần học theo ngày
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetWeekByDate")]
        [AppAuthorize]
        [Description("Lấy tuần học theo ngày")]
        public async Task<AppDomainResult> GetWeekByDate([FromQuery] WeekByDateSearch itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var schoolYear = await this.schoolYearService.GetByIdAsync(itemModel.schoolYearId ?? Guid.Empty);
            if (schoolYear == null)
                throw new AppException("Năm học không tồn tại!");
            var semester = await this.semesterService.GetByIdAsync(itemModel.semesterId ?? Guid.Empty);
            if (semester == null)
                throw new AppException("Học kỳ không tồn tại!");
            var item = await this.domainService.GetSingleAsync(x =>
            x.sTime <= itemModel.date &&
            x.eTime >= itemModel.date &&
            x.deleted == false &&
            x.schoolYearId == itemModel.schoolYearId &&
            x.semesterId == itemModel.semesterId
            );
            if (item == null)
            {
                if (itemModel.date < schoolYear.sTime)
                    throw new AppException("Năm học chưa bắt đầu");
                if (itemModel.date > schoolYear.eTime)
                    throw new AppException("Năm học đã kết thúc");
            }
            return new AppDomainResult(item);
        }
        /// <summary>
        /// Lấy danh sách tuần đã học
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetListWeekPastToNow")]
        [AppAuthorize]
        [Description("Lấy danh sách tuần đã học")]
        public async Task<AppDomainResult> GetListWeekPastToNow([FromQuery] WeekByDateSearch itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var schoolYear = await this.schoolYearService.GetByIdAsync(itemModel.schoolYearId ?? Guid.Empty);
            if (schoolYear == null)
                throw new AppException("Năm học không tồn tại!");
            var semester = await this.semesterService.GetByIdAsync(itemModel.semesterId ?? Guid.Empty);
            if (semester == null)
                throw new AppException("Học kỳ không tồn tại!");
            PagedList<tbl_Week> pagedList = new PagedList<tbl_Week>();
            IList<tbl_Week> data = new List<tbl_Week>();
            var item = await this.domainService.GetSingleAsync(x =>
            x.sTime <= itemModel.date &&
            x.eTime >= itemModel.date &&
            x.deleted == false &&
            x.schoolYearId == itemModel.schoolYearId &&
            x.semesterId == itemModel.semesterId
            );
            if (item == null)
            {
                if (itemModel.date < schoolYear.sTime)
                    throw new AppException("Năm học chưa bắt đầu");
                if (itemModel.date > schoolYear.eTime)
                    throw new AppException("Năm học đã kết thúc");
            }
            else
            {
                DateTime date = DateTimeOffset.FromUnixTimeMilliseconds((long)itemModel.date).UtcDateTime.ToLocalTime();
                item.name = item.name + " (" + date.ToString("dd/MM/yyyy") + ")";
                data = await this.domainService.GetAsync(x => x.deleted == false && x.schoolYearId == item.schoolYearId && x.semesterId == item.semesterId && x.weekNumber < item.weekNumber);
                data.Add(item);
            }
            pagedList.totalItem = data.Count;
            pagedList.items = data;
            pagedList.pageSize = data.Count;
            pagedList.pageIndex = 1;
            return new AppDomainResult(pagedList);
        }
        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] WeekCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_Week>(itemModel);
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            await Validate(item);
            var data = await this.domainService.GetAsync(x => x.schoolYearId == item.schoolYearId && x.semesterId == item.semesterId && x.deleted == false);
            int weekNumber = data.Count > 0 ? data.Max(x => x.weekNumber).Value + 1 : 1;
            var week = new tbl_Week()
            {
                name = item.name,
                schoolYearId = item.schoolYearId,
                weekNumber = weekNumber,
                semesterId = item.semesterId,
                sTime = item.sTime,
                eTime = item.eTime,
            };
            return new AppDomainResult(await this.weekService.AddWeek(week));
        }

        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut]
        [AppAuthorize]
        [Description("Chỉnh sửa")]
        public override async Task<AppDomainResult> UpdateItem([FromBody] WeekUpdate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_Week>(itemModel);

            if (item == null)
                throw new KeyNotFoundException(MessageContants.nf_item);
            await Validate(item);
            return new AppDomainResult(await this.weekService.UpdateWeek(item));
        }
    }

}
