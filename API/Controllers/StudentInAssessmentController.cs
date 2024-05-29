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

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Đánh giá trẻ theo chủ đề")]
    [Authorize]
    public class StudentInAssessmentController : BaseController<tbl_StudentInAssessment, StudentInAssessmentCreate, DomainUpdate, StudentInAssessmentSearch>
    {
        private readonly IStudentInAssessmentService studentInAssessmentService;
        private readonly ISemesterService semesterService;
        private readonly IChildAssessmentTopicService childAssessmentTopicService;
        private readonly IChildAssessmentDetailService childAssessmentDetailService;
        private readonly IParentService parentService;
        private readonly IStudentService studentService;
        public StudentInAssessmentController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_StudentInAssessment, StudentInAssessmentCreate, DomainUpdate, StudentInAssessmentSearch>> logger
            , IWebHostEnvironment env
            , IDomainHub hubcontext) : base(serviceProvider, logger, env
            )
        {
            this.domainService = serviceProvider.GetRequiredService<IStudentInAssessmentService>();
            this.studentInAssessmentService = serviceProvider.GetRequiredService<IStudentInAssessmentService>();
            this.semesterService = serviceProvider.GetRequiredService<ISemesterService>();
            this.childAssessmentTopicService = serviceProvider.GetRequiredService<IChildAssessmentTopicService>();
            this.childAssessmentDetailService = serviceProvider.GetRequiredService<IChildAssessmentDetailService>();
            this.parentService = serviceProvider.GetRequiredService<IParentService>();
            this.studentService = serviceProvider.GetRequiredService<IStudentService>();
        }
        [NonAction]
        public override Task<AppDomainResult> Get([FromQuery] StudentInAssessmentSearch baseSearch)
        {
            return base.Get(baseSearch);
        }
        [NonAction]
        public override Task<AppDomainResult> GetById(Guid id)
        {
            return base.GetById(id);
        }
        [NonAction]
        public override Task<AppDomainResult> DeleteItem(Guid id)
        {
            return base.DeleteItem(id);
        }
        [NonAction]
        public override Task<AppDomainResult> AddItem([FromBody] StudentInAssessmentCreate itemModel)
        {
            return base.AddItem(itemModel);
        }
        [NonAction]
        public override Task<AppDomainResult> UpdateItem([FromBody] DomainUpdate itemModel)
        {
            return base.UpdateItem(itemModel);
        }
        /// <summary>
        /// Danh sách học giá đánh giá theo chủ đề 
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize]
        [Description("Danh sách học giá đánh giá theo chủ đề")]
        public async Task<AppDomainResult> GetStudentInClassForAssessment([FromQuery] StudentInClassForAssessmentSearch itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.studentInAssessmentService.GetStudentInClassForAssessment(itemModel);
            return new AppDomainResult(data);
        }
        /// <summary>
        /// Cập nhật đánh giá trẻ theo chủ đề        
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Cập nhật đánh giá trẻ theo chủ đề")]
        public async Task<AppDomainResult> CreateOrUpdateAssessment([FromBody] StudentInAssessmentCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            bool success = true;
            foreach (var item in itemModel.studentModels)
            {
                var semester = await this.semesterService.GetByIdAsync(item.semesterId.Value);
                if (semester == null)
                    throw new AppException("Học kỳ không tồn tại!");
                var topic = await this.childAssessmentTopicService.GetByIdAsync(item.assessmentTopicId.Value);
                if (topic == null)
                    throw new AppException("Chủ đề đánh giá không tồn tại!");
                var topicDetail = await this.childAssessmentDetailService.GetAsync(x => x.childAssessmentId == item.assessmentTopicId.Value && x.deleted == false);
                foreach (var detail in topicDetail)
                {
                    bool? status = false;
                    if (itemModel.detailTopicModels.FirstOrDefault(x => x.assessmentDetailId == detail.id) != null)
                        status = true;
                    var data = new tbl_StudentInAssessment()
                    {
                        studentId = item.studentId.Value,
                        semesterId = item.semesterId.Value,
                        assessmentTopicId = item.assessmentTopicId.Value,
                        assessmentDetailId = detail.id,
                        status = status,
                    };
                    //var topicDetail = await this.childAssessmentDetailService.GetSingleAsync(x => x.id == detail.assessmentDetailId.Value && x.childAssessmentId == item.assessmentTopicId.Value && x.deleted == false);
                    if (topicDetail == null)
                        throw new AppException("Nội dung đánh giá không tồn tại!");
                    var studentInAssessment = await this.domainService.GetSingleAsync(x =>
                    x.assessmentTopicId == data.assessmentTopicId
                    && x.assessmentDetailId == data.assessmentDetailId
                    && x.semesterId == x.semesterId
                    && x.studentId == data.studentId
                    && x.deleted == false);
                    if (studentInAssessment != null)
                    {
                        data.id = studentInAssessment.id;
                        success = await this.studentInAssessmentService.UpdateAsync(data);
                    }
                    else
                    {
                        success = await this.studentInAssessmentService.CreateAsync(data);
                        // Thông tin phụ huynh
                        List<Guid> studentIds = new List<Guid>();
                        studentIds.Add(data.studentId.Value);
                        var parent = await parentService.GetParentUserByStudentId(studentIds);
                        var student = await this.studentService.GetByIdAsync(data.studentId ?? Guid.Empty);
                        // Thông báo cho phụ huynh
                        await studentInAssessmentService.SendNotify(topic, student, parent);
                    }
                    if (!success)
                        throw new AppException("Lỗi trong quá trình xử lý!");
                }
            }
            return new AppDomainResult()
            {
                resultCode = (int)HttpStatusCode.OK,
                resultMessage = "Cập nhật thành công!",
                success = success
            };
        }

        [HttpGet("assessment-result")]
        [AppAuthorize]
        [Description("Kết quả đánh giá của trẻ")]
        public async Task<AppDomainResult> AllCritialByStudent([FromQuery] CriteriaResult itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());

            var data = await this.studentInAssessmentService.AllCritialByStudent(itemModel);

            return new AppDomainResult(data);
        }
    }
}
