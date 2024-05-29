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
    [Description("Màn hình học kỳ")]
    [Authorize]
    public class SemesterController : BaseController<tbl_Semester, SemesterCreate, SemesterUpdate, SemesterSearch>
    {
        private readonly ISchoolYearService schoolYearService;
        public SemesterController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Semester, SemesterCreate, SemesterUpdate, SemesterSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.schoolYearService = serviceProvider.GetRequiredService<ISchoolYearService>();
            this.domainService = serviceProvider.GetRequiredService<ISemesterService>();
        }
        [NonAction]
        public override async Task Validate(tbl_Semester model)
        {
            if (model.schoolYearId.HasValue)
            {
                var item = await schoolYearService.GetByIdAsync(model.schoolYearId ?? Guid.Empty) ?? throw new AppException(MessageContants.nf_schoolYear);
            }
            if (model.semester.HasValue)
            {
                var item = await domainService.AnyAsync(x => x.schoolYearId == model.schoolYearId && x.semester == model.semester && x.deleted == false);
                if (item)
                    throw new AppException(MessageContants.exs_semester);
            }
            if (model.sTime.HasValue && model.eTime.HasValue)
            {
                if (model.sTime > model.eTime)
                    throw new AppException("Thời gian bắt đầu học kỳ không được phép nhỏ hơn hoặc bằng thời gian kết thúc!");
                var semester = await domainService.GetByIdAsync(model.id);
                if (semester != null)
                {
                    var beforeSemester = await domainService.GetSingleAsync(x => x.schoolYearId == semester.schoolYearId && x.semester == model.semester - 1 && x.deleted == false);
                    if (beforeSemester != null)
                    {
                        if (model.sTime <= beforeSemester.eTime)
                            throw new AppException("Thời gian bắt đầu học kỳ " + model.semester + " phải nhỏ hơn thời gian kết thúc học kỳ " + (model.semester - 1) + "!");
                    }
                    var afterSemester = await domainService.GetSingleAsync(x => x.schoolYearId == semester.schoolYearId && x.semester == model.semester + 1 && x.deleted == false);
                    if (afterSemester != null)
                    {
                        if (model.eTime >= afterSemester.sTime)
                            throw new AppException("Thời gian kết thúc học kỳ " + model.semester + " phải nhỏ hơn thời gian bắt đầu học kỳ " + (model.semester + 1) + "!");
                    }
                }
            }
        }
        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] SemesterCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_Semester>(itemModel);
            var semester = await domainService.CountAsync(x => x.deleted == false && x.schoolYearId == item.schoolYearId) + 1;
            item.semester = semester;
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            await Validate(item);
            await this.domainService.AddItem(item);
            return new AppDomainResult(item);
        }

        /// <summary>
        /// Lấy thông tin học kỳ
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSemesterByDate")]
        [AppAuthorize]
        [Description("Lấy thông tin học kỳ")]
        public virtual async Task<AppDomainResult> GetSemesterByDate([FromQuery] SemesterByDateSearch itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var schoolYear = await this.schoolYearService.GetByIdAsync(itemModel.schoolYearId.Value);
            if (schoolYear == null)
                throw new AppException("Năm học không tồn tại!");
            if (itemModel.date < schoolYear.sTime)
                return new AppDomainResult("Chưa bắt đầu!");
            if (itemModel.date > schoolYear.eTime)
                return new AppDomainResult("Đã kết thúc!");
            var item = await this.domainService.GetSingleAsync(x => x.schoolYearId == itemModel.schoolYearId
            && x.sTime <= itemModel.date && x.eTime >= itemModel.date);
            if (item == null)
                throw new AppException("Không tìm thấy học kỳ!");
            return new AppDomainResult(item);
        }
    }
}
