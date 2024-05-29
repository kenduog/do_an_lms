using AppDbContext;
using BaseAPI.Controllers;
using Entities;
using Entities.Search;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Request.RequestCreate;
using Request.RequestUpdate;
using Service.Services;
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
    [Description("Phân công giảng dạy")]
    [ApiController]
    public class TeachingAssignmentController : BaseController<tbl_TeachingAssignment, TeachingAssignmentCreate, TeachingAssignmentUpdate, TeachingAssignmentSearch>
    {
        private readonly ITeachingAssignmentService teachingAssignmentService;
        public TeachingAssignmentController(IServiceProvider serviceProvider
            , IAppDbContext appDbContext
            , ILogger<BaseController<tbl_TeachingAssignment, TeachingAssignmentCreate, TeachingAssignmentUpdate, TeachingAssignmentSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<ITeachingAssignmentService>();
            this.teachingAssignmentService = serviceProvider.GetRequiredService<ITeachingAssignmentService>();
        }

        [NonAction]
        public override Task<AppDomainResult> UpdateItem([FromBody] TeachingAssignmentUpdate itemModel) => base.UpdateItem(itemModel);

        /// <summary>
        /// Thêm mới 
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới một phân công giảng dạy")]
        public override async Task<AppDomainResult> AddItem([FromBody] TeachingAssignmentCreate itemModel)
        {
            await teachingAssignmentService.AddOrUpdate(itemModel);
            return new AppDomainResult();
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]   
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public override async Task<AppDomainResult> Get([FromQuery] TeachingAssignmentSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());

            PagedList<tbl_TeachingAssignment> pagedData = await this.domainService.GetPagedListData(baseSearch);

            return new AppDomainResult(pagedData);
        }

        /// <summary>
        /// Get teachers by subject
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet("by-subject")]
        [AppAuthorize]
        [Description("Get teacher by subject")]
        public async Task<AppDomainResult> PrepareToCreateTimeTable([FromQuery] TeacherBySubjectRequest baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            if (baseSearch == null) baseSearch = new TeacherBySubjectRequest();
            List<TeacherBySubjectReponse> data = await this.teachingAssignmentService.GetTeacherAssignmentBySubject(baseSearch);
            return new AppDomainResult(data);
        }
    }
}
