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
using BaseAPI.Controllers;
using Interface.DbContext;
using System.Data.Entity;
using Microsoft.AspNetCore.Http;
using Entities.AuthEntities;
using AppDbContext;
using OfficeOpenXml.ThreadedComments;
using static Utilities.CoreContants;
using Service.Services;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Description("phụ huynh")]
    [Authorize]
    public class ParentController : BaseController<tbl_Parent, ParentCreate, ParentUpdate, ParentSearch>
    {
        private readonly IAppDbContext appDbContext;
        private readonly IUserService userService;
        private readonly IGroupService groupService;
        private readonly IUserGroupService userGroupService;
        private readonly IBranchService branchService;
        private readonly IStudentService studentService;
        private readonly IParentService parentService;
        private readonly ICitiesService citiesService;
        private readonly IDistrictsService districtsService;
        private readonly IWardsService wardsService;
        private readonly ISendNotificationService sendNotificationService;
        private readonly INecessaryService necessaryService;
        public ParentController(IServiceProvider serviceProvider, ILogger<BaseController<tbl_Parent, ParentCreate, ParentUpdate, ParentSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.appDbContext = serviceProvider.GetRequiredService<IAppDbContext>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.domainService = serviceProvider.GetRequiredService<IParentService>();
            this.groupService = serviceProvider.GetRequiredService<IGroupService>();
            this.userGroupService = serviceProvider.GetRequiredService<IUserGroupService>();
            this.branchService = serviceProvider.GetRequiredService<IBranchService>();
            this.studentService = serviceProvider.GetRequiredService<IStudentService>();
            this.parentService = serviceProvider.GetRequiredService<IParentService>();
            this.citiesService = serviceProvider.GetRequiredService<ICitiesService>();
            this.districtsService = serviceProvider.GetRequiredService<IDistrictsService>();
            this.wardsService = serviceProvider.GetRequiredService<IWardsService>();
            this.sendNotificationService = serviceProvider.GetService<ISendNotificationService>();
            this.necessaryService = serviceProvider.GetService<INecessaryService>();
        }

        [HttpGet("{id}")]
        [AppAuthorize]
        [Description("Lấy thông tin")]
        public override async Task<AppDomainResult> GetById(Guid id)
        {
            var item = await this.domainService.GetByIdAsync(id) ?? throw new KeyNotFoundException(MessageContants.nf_item);
            var user = await userService.GetByIdAsync(item.userId.Value);
            var city = await citiesService.GetByIdAsync(user.cityId ?? Guid.Empty);
            var district = await districtsService.GetByIdAsync(user.districtId ?? Guid.Empty);
            var ward = await wardsService.GetByIdAsync(user.wardId ?? Guid.Empty);
            item = new tbl_Parent
            {
                id = item.id,
                active = item.active,
                address = user.address,
                birthday = user.birthday,
                branchIds = user.branchIds,
                cityId = user.cityId,
                cityName = city?.name,
                code = user.code,
                created = item.created,
                createdBy = item.createdBy,
                deleted = item.deleted,
                districtId = user.districtId,
                districtName = district?.name,
                email = user.email,
                firstName = user.firstName,
                fullName = user.fullName,
                gender = user.gender,
                genderName = user.genderName,
                job = item.job,
                lastName = user.lastName,
                note = item.note,
                phone = user.phone,
                status = user.status,
                statusName = user.statusName,
                thumbnail = user.thumbnail,
                thumbnailResize = user.thumbnailResize,
                type = item.type,
                typeName = item.typeName,
                updated = item.updated,
                updatedBy = item.updatedBy,
                userId = item.userId,
                username = user.username,
                wardId = user.wardId,
                wardName = ward?.name,
                students = Task.Run(() => parentService.GetStudentByParent(item.id)).Result
            };
            return new AppDomainResult(item);
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize]
        [Description("Thêm mới")]
        [NonAction]
        public override async Task<AppDomainResult> AddItem([FromBody] ParentCreate itemModel)
        {
            using (var tran = await appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (!ModelState.IsValid)
                        throw new AppException(ModelState.GetErrorMessage());

                    //Thông tin người dùng
                    var user = mapper.Map<tbl_Users>(itemModel);
                    await userService.ValidateUser(user);
                    await userService.CreateAsync(user);

                    var item = mapper.Map<tbl_Parent>(itemModel);
                    if (item == null)
                        throw new AppException(MessageContants.nf_item);
                    item.userId = user.id;
                    await Validate(item);
                    await this.domainService.AddItem(item);

                    var parentGroup = await groupService.GetByCode("PH");
                    if (parentGroup == null)
                        throw new AppException("Không tìm thấy phân quyền phụ huynh");
                    var student = await studentService.GetByIdAsync(itemModel.studentId.Value);
                    if (student == null)
                        throw new AppException("Không tìm thấy thông tin học viên");
                    if (item.type == 1) student.fatherId = item.id;
                    else if (item.type == 2) student.motherId = item.id;
                    else student.guardianId = item.id;
                    await studentService.UpdateAsync(student);
                    var userGroup = new tbl_UserGroup
                    {
                        active = true,
                        created = Timestamp.Now(),
                        deleted = true,
                        groupId = parentGroup.id,
                        updated = Timestamp.Now(),
                        userId = user.id,
                    };
                    await userGroupService.CreateAsync(userGroup);
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

        [HttpPut]
        [AppAuthorize]
        [Description("Chỉnh sửa")]
        public override async Task<AppDomainResult> UpdateItem([FromBody] ParentUpdate itemModel)
        {
            using (var tran = await appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (!ModelState.IsValid)
                        throw new AppException(ModelState.GetErrorMessage());
                    var student = new tbl_Student();
                    if (itemModel.studentId.HasValue)
                    {
                        student = studentService.GetById(itemModel.studentId.Value);
                        if (student == null)
                            throw new AppException("Không tìm thấy thông tin học viên");
                        if ((student.fatherId.HasValue && itemModel.type == 1)
                        || (student.motherId.HasValue && itemModel.type == 2)
                        || (student.guardianId.HasValue && itemModel.type == 3))
                            throw new AppException($"Đã tồn tại thông tin {itemModel.typeName}");
                    }
                    var parent = domainService.GetById(itemModel.id);
                    if (parent == null)
                    {
                        if (string.IsNullOrEmpty(itemModel.code))
                            throw new AppException("Vui lòng nhập mã người dùng");
                        if (string.IsNullOrEmpty(itemModel.username))
                            throw new AppException("Vui lòng nhập tài khoản đăng nhập");
                        if (string.IsNullOrEmpty(itemModel.fullName))
                            throw new AppException("Vui lòng nhập họ và tên");
                        if (string.IsNullOrEmpty(itemModel.branchIds))
                            throw new AppException("Vui lòng chọn chi nhánh");
                        if (!itemModel.type.HasValue)
                            throw new AppException("Vui lòng chọn loại");

                        var user = new tbl_Users
                        {
                            active = true,
                            address = itemModel.address,
                            birthday = itemModel.birthday,
                            branchIds = itemModel.branchIds,
                            cityId = itemModel.cityId,
                            code = itemModel.code,
                            created = Timestamp.Now(),
                            deleted = false,
                            districtId = itemModel.districtId,
                            email = itemModel.email,
                            firstName = itemModel.firstName,
                            genderName = itemModel.genderName,
                            fullName = itemModel.fullName,
                            gender = itemModel.gender,
                            isSuperUser = false,
                            lastName = itemModel.lastName,
                            password = itemModel.password,
                            phone = itemModel.phone,
                            status = ((int)UserStatus.Active),
                            statusName = itemModel.statusName,
                            thumbnail = itemModel.thumbnail,
                            thumbnailResize = itemModel.thumbnailResize,
                            updated = Timestamp.Now(),
                            username = itemModel.username,
                            wardId = itemModel.wardId,
                        };
                        await userService.ValidateUser(user);
                        await userService.CreateAsync(user);

                        var item = new tbl_Parent
                        {
                            active = true,
                            created = Timestamp.Now(),
                            deleted = false,
                            job = itemModel.job,
                            note = itemModel.note,
                            type = itemModel.type,
                            typeName = itemModel.typeName,
                            updated = Timestamp.Now(),
                            userId = user.id,
                        };
                        await domainService.CreateAsync(item);
                        //if (item.type == 1) student.fatherId = item.id;
                        //else if (item.type == 2) student.motherId = item.id;
                        //else student.guardianId = item.id;
                        //await studentService.UpdateAsync(student);

                        var parentGroup = await groupService.GetByCode("PH");
                        if (parentGroup == null)
                            throw new AppException("Không tìm thấy phân quyền phụ huynh");
                        var userGroup = new tbl_UserGroup
                        {
                            active = true,
                            created = Timestamp.Now(),
                            deleted = true,
                            groupId = parentGroup.id,
                            updated = Timestamp.Now(),
                            userId = user.id,
                        };
                        await userGroupService.CreateAsync(userGroup);
                        // Gửi mail khi có phụ huynh mới được tạo
                        string title = "Thông báo tài khoản phụ huynh vừa được tạo mới";
                        StringBuilder content = new StringBuilder();
                        content.Append($"<p style=\"font-size:30px;\">Thông báo phụ huynh {user.fullName} vừa được thêm vào hệ thống</p>");
                        content.Append($"<p>Tài khoản: {user.username}</p>");
                        content.Append($"<p>mật khẩu: {itemModel.setPassword}</p>");
                        var sendMail = new Thread(async () =>
                        {
                            await necessaryService.SendMail(new SendMailModel
                            {
                                to = user.email,
                                title = title,
                                content = content.ToString()
                            });
                        });
                        sendMail.Start();
                    }
                    else
                    {
                        //Thông tin người dùng
                        var user = mapper.Map<tbl_Users>(itemModel);
                        user.id = parent.userId.Value;
                        await userService.ValidateUser(user);
                        await userService.UpdateAsync(user);

                        var item = mapper.Map<tbl_Parent>(itemModel);
                        if (item == null)
                            throw new KeyNotFoundException(MessageContants.nf_item);

                        if (itemModel.studentId.HasValue)
                        {
                            if (item.type == 1) student.fatherId = item.id;
                            else if (item.type == 2) student.motherId = item.id;
                            else student.guardianId = item.id;
                            await studentService.UpdateAsync(student);
                        }

                        await this.domainService.UpdateItem(item);
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
        /// <summary>
        /// Xóa item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AppAuthorize]
        [Description("Xoá")]
        [NonAction]
        public override async Task<AppDomainResult> DeleteItem(Guid id)
        {
            using (var tran = await appDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var parent = await domainService.GetByIdAsync(id);
                    if (parent == null)
                        throw new AppException("Không tìm thấy thông tin người dùng");
                    await userService.DeleteItem(parent.userId.Value);
                    await this.domainService.DeleteItem(id);
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

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize]
        [Description("Lấy danh sách")]
        public override async Task<AppDomainResult> Get([FromQuery] ParentSearch baseSearch)
        {
            if (!ModelState.IsValid)
                throw new AppException(ModelState.GetErrorMessage());
            var userId = LoginContext.Instance.CurrentUser.userId;
            var user = await userService.GetByIdAsync(userId);
            if (!(await userService.IsAdmin(userId)))
            {
                baseSearch.myBranchIds = user.branchIds;
            }
            PagedList<tbl_Parent> pagedData = await this.domainService.GetPagedListData(baseSearch);
            pagedData.items = (from i in pagedData.items
                               select new tbl_Parent
                               {
                                   id = i.id,
                                   active = i.active,
                                   address = i.address,
                                   birthday = i.birthday,
                                   branchIds = i.branchIds,
                                   cityId = i.cityId,
                                   cityName = i.cityName,
                                   code = i.code,
                                   created = i.created,
                                   createdBy = i.createdBy,
                                   deleted = i.deleted,
                                   districtId = i.districtId,
                                   districtName = i.districtName,
                                   email = i.email,
                                   firstName = i.firstName,
                                   fullName = i.fullName,
                                   gender = i.gender,
                                   genderName = i.genderName,
                                   job = i.job,
                                   lastName = i.lastName,
                                   note = i.note,
                                   phone = i.phone,
                                   status = i.status,
                                   statusName = i.statusName,
                                   thumbnail = i.thumbnail,
                                   thumbnailResize = i.thumbnailResize,
                                   type = i.type,
                                   typeName = i.typeName,
                                   updated = i.updated,
                                   updatedBy = i.updatedBy,
                                   userId = i.userId,
                                   username = i.username,
                                   wardId = i.wardId,
                                   wardName = i.wardName,
                                   students = Task.Run(() => parentService.GetStudentByParent(i.id)).Result
                               }).ToList();

            return new AppDomainResult(pagedData);
        }

        [HttpGet("by-student/{studentId}")]
        [AppAuthorize]
        [Description("Lấy thông tin phụ huynh theo học viên")]
        public async Task<AppDomainResult> GetByStudent(Guid studentId)
        {
            var result = new ParentByStudent();
            var student = await studentService.GetByIdAsync(studentId);
            if (student == null)
                return new AppDomainResult(result);

            var types = new List<int> { 1, 2, 3 };
            foreach (var type in types)
            {
                Guid? parentId = type == 1 ? student.fatherId
                                : type == 2 ? student.motherId
                                : student.guardianId;
                if (parentId.HasValue)
                {
                    var item = await this.domainService.GetByIdAsync(parentId.Value);
                    var user = await userService.GetByIdAsync(item.userId.Value);
                    var city = await citiesService.GetByIdAsync(user.cityId ?? Guid.Empty);
                    var district = await districtsService.GetByIdAsync(user.districtId ?? Guid.Empty);
                    var ward = await wardsService.GetByIdAsync(user.wardId ?? Guid.Empty);
                    item = new tbl_Parent
                    {
                        id = item.id,
                        active = item.active,
                        address = user.address,
                        birthday = user.birthday,
                        branchIds = user.branchIds,
                        cityId = user.cityId,
                        cityName = city?.name,
                        code = user.code,
                        created = item.created,
                        createdBy = item.createdBy,
                        deleted = item.deleted,
                        districtId = user.districtId,
                        districtName = district?.name,
                        email = user.email,
                        firstName = user.firstName,
                        fullName = user.fullName,
                        gender = user.gender,
                        genderName = user.genderName,
                        job = item.job,
                        lastName = user.lastName,
                        note = item.note,
                        phone = user.phone,
                        status = user.status,
                        statusName = user.statusName,
                        thumbnail = user.thumbnail,
                        thumbnailResize = user.thumbnailResize,
                        type = item.type,
                        typeName = item.typeName,
                        updated = item.updated,
                        updatedBy = item.updatedBy,
                        userId = item.userId,
                        username = user.username,
                        wardId = user.wardId,
                        wardName = ward?.name,
                        students = Task.Run(() => parentService.GetStudentByParent(item.id)).Result
                    };
                    switch (type)
                    {
                        case 1: result.father = item; break;
                        case 2: result.mother = item; break;
                        case 3: result.guardian = item; break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case 1: result.father = new tbl_Parent { type = type }; break;
                        case 2: result.mother = new tbl_Parent { type = type }; break;
                        case 3: result.guardian = new tbl_Parent { type = type }; break;
                    }
                }
            }
            return new AppDomainResult(result);
        }
    }
}
