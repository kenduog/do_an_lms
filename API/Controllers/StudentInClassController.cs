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
using OfficeOpenXml.ConditionalFormatting;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Màn hình học viên của lớp học")]
    [Authorize]
    public class StudentInClassController : BaseController<tbl_StudentInClass, StudentInClassCreate, StudentInClassUpdate, StudentInClassSearch>
    {
        private readonly IClassService classService;
        private readonly IStudentService studentService;
        private readonly IAppDbContext appDbContext;
        private readonly IStudentInClassService studentInClassService;
        private readonly IAttendanceService attendanceService;
        public StudentInClassController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_StudentInClass, StudentInClassCreate, StudentInClassUpdate, StudentInClassSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env
            )
        {
            this.studentService = serviceProvider.GetRequiredService<IStudentService>();
            this.classService = serviceProvider.GetRequiredService<IClassService>();
            this.domainService = serviceProvider.GetRequiredService<IStudentInClassService>();
            this.appDbContext = serviceProvider.GetRequiredService<IAppDbContext>();
            this.studentInClassService = serviceProvider.GetRequiredService<IStudentInClassService>();
            this.attendanceService = serviceProvider.GetRequiredService<IAttendanceService>();
        }
        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        public override async Task<AppDomainResult> AddItem([FromBody] StudentInClassCreate itemModel)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_StudentInClass>(itemModel);
            if (item == null)
                throw new AppException(MessageContants.nf_item);
            var _class = await classService.GetByIdAsync(item.classId.Value);
            if (_class == null)
                throw new AppException("Không tìm thấy lớp học");
            var student = await studentService.GetByIdAsync(item.studentId.Value);
            if (student == null)
                throw new AppException("Không tìm thấy học viên");

            item.gradeId = _class.gradeId;
            item.schoolYearId = _class.schoolYearId;
            bool hasClass = await domainService.AnyAsync(x => x.studentId == itemModel.studentId && x.classId == item.classId && x.deleted == false);
            if (hasClass)
                throw new AppException("Học viên đang học lớp này");
            await this.domainService.AddItem(item);
            student.gradeId = item.gradeId;
            await studentService.UpdateAsync(student);
            
            return new AppDomainResult(item);
        }
        [HttpPost("multiple")]
        [AppAuthorize]
        [Description("Thêm mới")]
        public async Task<AppDomainResult> MultipleAddItem([FromBody] MultipleStudentInClassCreate itemModel)
        {
            using (var tran = await appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (!ModelState.IsValid)
                        throw new AppException(ModelState.GetErrorMessage());
                    var _class = await classService.ClassByIdAsync(itemModel.classId ?? Guid.Empty);
                    if (_class == null)
                        throw new AppException("Không tìm thấy lớp học");
                    if (!itemModel.studentIds.Any())
                        throw new AppException("Vui lòng chọn học viên");
                    foreach (var studentId in itemModel.studentIds)
                    {

                        var student = studentService.GetById(studentId);
                        if (student == null)
                            throw new AppException("Không tìm thấy học viên");
                        bool hasClass = await domainService.AnyAsync(x => x.studentId == studentId && x.classId == _class.id && x.deleted == false);
                        if (hasClass)
                            throw new AppException($"Học viên {student.fullName} đang học lớp này");
                        var item = new tbl_StudentInClass
                        {
                            active = true,
                            classId = _class.id,
                            created = Timestamp.Now(),
                            deleted = true,
                            gradeId = _class.gradeId,
                            note = itemModel.note,
                            schoolYearId = _class.schoolYearId,
                            status = 1,
                            statusName = tbl_StudentInClass.GetStatusName(1),
                            studentId = studentId,
                            updated = Timestamp.Now()
                        };

                        item.gradeId = _class.gradeId;
                        item.schoolYearId = _class.schoolYearId;
                        await this.domainService.AddItem(item);

                        student.gradeId = item.gradeId;
                        await attendanceService.GenerateAttendance(_class, studentId);
                        await studentService.UpdateAsync(student);
                        await studentInClassService.SendNotification(student, _class);
                    }
                    await tran.CommitAsync();
                    return new AppDomainResult();
                }
                catch (AppException e)
                {
                    await tran.RollbackAsync();
                    throw new AppException(e.Message); 
                }
            }
        }

        [HttpGet("student-available")]
        [AppAuthorize]
        [Description("Lấy danh sách học viên khi thêm vào lớp")]
        public async Task<AppDomainResult> GetStudentAvailable([FromQuery] StudentAvailableSearch baseSearch)
        {
            var data = await studentInClassService.GetStudentAvailable(baseSearch);
            return new AppDomainResult(data);
        }

    }
}
