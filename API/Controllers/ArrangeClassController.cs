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
using System.Transactions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance.Implementations;
using static Utilities.CoreContants;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("Màn hình xếp lớp đầu năm")]
    [Authorize]
    public class ArrangeClassController : ControllerBase
    {
        private readonly IClassService classService;
        private readonly IStudentService studentService;
        private readonly IMapper mapper;
        private readonly IArrangeClassService arrangeClassService;
        private readonly IAppDbContext appDbContext;
        private readonly IStudentInClassService studentInClassService;
        private readonly IGradeService gradeService;
        private readonly ISchoolYearService schoolYearService;
        private readonly IStaffService staffService;
        private readonly IBranchService branchService;
        public ArrangeClassController(IServiceProvider serviceProvider, IMapper mapper)
        {
            this.studentService = serviceProvider.GetRequiredService<IStudentService>();
            this.classService = serviceProvider.GetRequiredService<IClassService>();
            this.mapper = mapper;
            this.arrangeClassService = serviceProvider.GetRequiredService<IArrangeClassService>();
            this.appDbContext = serviceProvider.GetRequiredService<IAppDbContext>();
            this.studentInClassService = serviceProvider.GetRequiredService<IStudentInClassService>();
            this.gradeService = serviceProvider.GetRequiredService<IGradeService>();
            this.schoolYearService = serviceProvider.GetRequiredService<ISchoolYearService>();
            this.staffService = serviceProvider.GetRequiredService<IStaffService>();
            this.branchService = serviceProvider.GetRequiredService<IBranchService>();
        }

        [HttpPost("arrange")]
        [AppAuthorize]
        [Description("Xếp lớp cho học sinh")]
        public async Task<AppDomainResult> Arrange([FromBody] StudentInClassCreate itemModel)
        {

            return new AppDomainResult { success = true, resultCode = (int)HttpStatusCode.OK, resultMessage = MessageContants.success };
        }

        /// <summary>
        /// Tạo lớp mới
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        [HttpPost("class")]
        [AppAuthorize]
        [Description("Tạo lớp mới")]
        public async Task<AppDomainResult> GetClasses([FromBody] ClassCreate itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var item = mapper.Map<tbl_Class>(itemModel);
            if (item == null)
                throw new AppException(MessageContants.nf_item);

            success = await this.classService.CreateAsync(item);
            if (!success)
                throw new AppException(MessageContants.err);

            appDomainResult.success = success;
            appDomainResult.resultCode = (int)HttpStatusCode.OK;
            appDomainResult.resultMessage = MessageContants.success;

            return new AppDomainResult { success = true, resultCode = (int)HttpStatusCode.OK, resultMessage = MessageContants.success };
        }

        /// <summary>
        /// Lấy danh sách lớp (có filter)
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("classes")]
        [AppAuthorize]
        [Description("Lấy thông danh sách lớp")]
        public async Task<AppDomainResult> GetClasses([FromBody] ClassSearch search)
        {
            var data = await this.classService.GetPagedListData(search);
            return new AppDomainResult { success = true, resultCode = (int)HttpStatusCode.OK, data = data };
        }

        /// <summary>
        /// Lấy danh sách học sinh 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("students")]
        [AppAuthorize]
        [Description("Lấy thông danh sách học sinh")]
        public async Task<AppDomainResult> GetStudents(ArrangeNewClassSearch search)
            => await this.studentService.GetStudentsForArrange(search);

        [HttpGet("student-when-arrange-class")]
        [AppAuthorize]
        [Description("Lấy danh sách học viên khi xếp lớp")]
        public async Task<AppDomainResult> GetStudentWhenArrangeClass([FromQuery] StudentWhenArrangeClassSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var data = await this.arrangeClassService.GetStudentWhenArrangeClass(baseSearch);

            int totalPage = 0;
            decimal count = data.Item1;
            if (count > 0)
                totalPage = (int)Math.Ceiling(count / baseSearch.pageSize);
            var pagedData = new
            {
                items = data.Item2,
                pageIndex = baseSearch.pageIndex,
                pageSize = baseSearch.pageSize,
                totalItem = data.Item1,
                totalPage = totalPage
            };
            return new AppDomainResult(pagedData);
        }
        [HttpPost("arrange-class")]
        [AppAuthorize]
        [Description("Xếp lớp cho học viên")]
        public async Task<AppDomainResult> ArrangeClass([FromBody] ArrangeClassRequest itemModel)
        {
            using (var tran = await appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (!ModelState.IsValid)
                        throw new AppException(ModelState.GetErrorMessage());

                    var hasSchoolYear = await schoolYearService.AnyAsync(x => x.id == itemModel.schoolYearId);
                    if (!hasSchoolYear)
                        throw new AppException(MessageContants.nf_schoolYear);
                    var hasBranch = await branchService.AnyAsync(x => x.id == itemModel.branchId.Value);
                    if (!hasBranch)
                        throw new AppException(MessageContants.nf_branch);
                    if (!itemModel.classes.Any())
                        throw new AppException(MessageContants.nf_class);

                    foreach (var item in itemModel.classes)
                    {
                        var _class = classService.GetById(item.classId);
                        if (_class == null)
                            throw new AppException(MessageContants.nf_class);
                        if (_class.size < item.studentIds.Count())
                            throw new AppException($"Lớp {_class.name} không đủ chỗ");

                        //var hasGrade = await gradeService.AnyAsync(x => x.id == item.gradeId);
                        //if (!hasGrade)
                        //    throw new AppException("Không tìm thấy khối");
                        //var teacher = await staffService.GetByIdAsync(item.teacherId.Value);
                        //if (teacher == null)
                        //    throw new AppException("Không tìm thấy giáo viên");

                        //var _class = new tbl_Class
                        //{
                        //    active = true,
                        //    created = Timestamp.Now(),
                        //    deleted = false,
                        //    gradeId = item.gradeId,
                        //    name = item.name,
                        //    schoolYearId = itemModel.schoolYearId,
                        //    size = item.size,
                        //    updated = Timestamp.Now(),
                        //    teacherId = teacher.id,
                        //    branchId = itemModel.branchId
                        //};
                        //await classService.CreateAsync(_class);
                        foreach (var studentId in item.studentIds)
                        {
                            var student = studentService.GetById(studentId);
                            if (student == null)
                                throw new AppException(MessageContants.nf_student);
                            var studentInClass = new tbl_StudentInClass
                            {
                                active = true,
                                classId = _class.id,
                                created = Timestamp.Now(),
                                deleted = false,
                                gradeId = _class.gradeId,
                                schoolYearId = _class.schoolYearId,
                                status = 1,
                                statusName = tbl_StudentInClass.GetStatusName(1),
                                studentId = studentId,
                                updated = Timestamp.Now()
                            };
                            student.gradeId = _class.gradeId;
                            student.status = (int)StudentStatus.DangHoc;
                            student.statusName = GetStudentStatusName((int)StudentStatus.DangHoc);
                            await studentInClassService.CreateAsync(studentInClass);
                            await studentService.UpdateAsync(student);
                            await studentInClassService.SendNotification(student, _class);
                        }
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
    }
}
