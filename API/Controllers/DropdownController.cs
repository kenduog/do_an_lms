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
using static Utilities.CoreContants;
using Microsoft.AspNetCore.Http;
using Entities.DomainEntities;
using Service.Services;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Description("Dropdown")]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class DropdownController : ControllerBase
    {
        protected IDropdownService dropdownService;
        protected IGradeService gradeService;
        protected IClassService classService;

        public DropdownController(IServiceProvider serviceProvider, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.dropdownService = serviceProvider.GetRequiredService<IDropdownService>();
            this.gradeService = serviceProvider.GetRequiredService<IGradeService>();
            this.classService = serviceProvider.GetRequiredService<IClassService>();
        }

        [HttpGet("group")]
        [AppAuthorize]
        [Description("Lấy danh sách nhóm quyền")]
        public async Task<AppDomainResult> GroupOption([FromQuery] GroupOptionSearch search)
        {
            var data = await dropdownService.GroupOption(search);
            return new AppDomainResult(data);
        }

        [HttpGet("group-staff")]
        [AppAuthorize]
        [Description("Lấy danh sách nhóm quyền nhân viên")]
        public async Task<AppDomainResult> GroupStaffOption()
        {
            var data = await dropdownService.GroupStaffOption();
            return new AppDomainResult(data);
        }

        [HttpGet("feedback-group-staff")]
        [AppAuthorize]
        [Description("Lấy danh sách nhóm quyền nhân viên")]
        public async Task<AppDomainResult> FeedbackGroupStaffOption()
        {
            var data = await dropdownService.FeedbackGroupStaffOption();
            return new AppDomainResult(data);
        }

        [HttpGet("school-year")]
        [AllowAnonymous]
        [Description("Lấy danh sách năm học")]
        public async Task<AppDomainResult> SchoolYearOption()
        {
            var data = await dropdownService.SchoolYearOption();
            return new AppDomainResult(data);
        }

        [HttpGet("class/{gradeId}")]
        [AppAuthorize]
        [Description("Lấy danh sách lớp")]
        public async Task<AppDomainResult> ClassOption(Guid gradeId)
        {
            var data = await dropdownService.ClassOption(gradeId);
            return new AppDomainResult(data);
        }

        [HttpGet("class/{grade}/{id}")]
        [AppAuthorize]
        [Description("Lấy danh sách lớp theo id")]
        public async Task<AppDomainResult> ClassOption(Guid grade, Guid id)
        {
            var classItem = await classService.GetByIdAsync(id);
            var list = new List<DomainOption>();
            list.Add(new DomainOption()
            {
                id = classItem.id,
                name = classItem.name,
            });
            return new AppDomainResult(list);
        }

        [HttpGet("grade/{branchId}")]
        [AppAuthorize]
        [Description("Lấy danh sách khối lớp")]
        public async Task<AppDomainResult> GradeOption(Guid branchId)
        {
            var data = await dropdownService.GradeOption(branchId);
            return new AppDomainResult(data);
        }

        [HttpGet("branch")]
        [AllowAnonymous]
        [Description("Lấy danh sách chi nhánh")]
        public async Task<AppDomainResult> BranchOption()
        {
            var data = await dropdownService.BranchOption();
            return new AppDomainResult(data);
        }

        [HttpGet("semester/{schoolYearId}")]
        [AllowAnonymous]
        [Description("Lấy danh sách học kỳ")]
        public async Task<AppDomainResult> SemesterOption(Guid schoolYearId)
        {
            var data = await dropdownService.SemesterOption(schoolYearId);
            return new AppDomainResult(data);
        }

        [HttpGet("department")]
        [AppAuthorize]
        [Description("Lấy danh sách phòng ban")]
        public async Task<AppDomainResult> DepartmentOption()
        {
            var data = await dropdownService.DepartmentOption();
            return new AppDomainResult(data);
        }

        [HttpGet("highest-level-of-education")]
        [AppAuthorize]
        [Description("Lấy danh sách học vấn")]
        public async Task<AppDomainResult> HighestLevelOfEducationOption()
        {
            var data = await dropdownService.HighestLevelOfEducationOption();
            return new AppDomainResult(data);
        }

        /// <summary>
        /// Lấy filter
        /// </summary>
        /// <returns></returns>
        [HttpGet("filter-study-shift")]
        [AppAuthorize]
        [Description("Lấy danh sách filter ca học")]
        public async Task<AppDomainResult> FilterOption()
        {
            var data = await dropdownService.StudyShiftFilterOption();
            return new AppDomainResult(data);
        }

        [HttpGet("staff-by-code")]
        [AppAuthorize]
        [Description("Lấy danh sách nhân viên theo mã")]
        public async Task<AppDomainResult> StaffOptionByCode([FromQuery] StaffOptionSearch search)
        {
            var data = await dropdownService.StaffOptionByCode(search);
            return new AppDomainResult(data);
        }

        [HttpGet("discount/{tuitionConfig}")]
        [AppAuthorize]
        [Description("Lấy danh sách khuyến mãi")]
        public async Task<AppDomainResult> DiscountOption(Guid tuitionConfig)
        {
            var data = await dropdownService.DiscountOption(tuitionConfig);
            return new AppDomainResult(data);
        }
        [HttpGet("tuition-config-category")]
        [AppAuthorize]
        [Description("Lấy danh sách loại khoản thu")]
        public async Task<AppDomainResult> TuitionConfigCategoryOption()
        {
            var data = await dropdownService.TuitionConfigCategoryOption();
            return new AppDomainResult(data);
        }
        [HttpGet("tuition-config")]
        [AppAuthorize]
        [Description("Lấy danh sách khoản thu")]
        public async Task<AppDomainResult> TuitionConfigOption()
        {
            var data = await dropdownService.TuitionConfigOption();
            return new AppDomainResult(data);
        }

        [HttpGet("student")]
        [AppAuthorize]
        [Description("Lấy danh sách học viên theo chi nhánh")]
        public async Task<AppDomainResult> StudentOption([FromQuery] StudentOptionSearch search)
        {
            var data = await dropdownService.StudentOption(search);
            return new AppDomainResult(data);
        }

        [HttpGet("subject-group/{branchId}")]
        [AppAuthorize]
        [Description("Lấy danh sách nhóm môn học")]
        public async Task<AppDomainResult> SubjectGroupOption(Guid branchId)
        {
            var data = await dropdownService.SubjectGroupOption(branchId);
            return new AppDomainResult(data);
        }

        [HttpGet("subject")]
        [AppAuthorize]
        [Description("Lấy danh sách môn học")]
        public async Task<AppDomainResult> SubjectOption([FromQuery] OptionSearch search)
        {
            var data = await dropdownService.SubjectOption(search);
            return new AppDomainResult(data);
        }

        [HttpGet("criteria/{gradeId}")]
        [AppAuthorize]
        [Description("Lấy danh sách lĩnh vực đánh giá theo grade")]
        public async Task<AppDomainResult> CriteriaOption(Guid gradeId)
        {
            var data = await dropdownService.CriteriaOption(gradeId);
            return new AppDomainResult(data);
        }

        [HttpGet("room/{branchId}")]
        [AppAuthorize]
        [Description("Danh sách phòng học theo trung tâp")]
        public async Task<AppDomainResult> RoomOption(Guid gradeId)
        {
            var data = await dropdownService.RoomOption(gradeId);
            return new AppDomainResult(data);
        }

        [HttpGet("week/{semesterId}")]
        [AppAuthorize]
        [Description("Danh sách tuần học theo học kì")]
        public async Task<AppDomainResult> WeekOption(Guid semesterId)
        {
            var data = await dropdownService.WeekOption(semesterId);
            return new AppDomainResult(data);
        }

        [HttpGet("unit-of-measure")]
        [AppAuthorize]
        [Description("Lấy danh sách don vi tinh")]
        public async Task<AppDomainResult> UnitOfMeasureOption()
        {
            var data = await dropdownService.UnitOfMeasureOption();
            return new AppDomainResult(data);
        }

        [HttpGet("item-group")]
        [AppAuthorize]
        [Description("Lấy danh sách nhom nguyen lieu")]
        public async Task<AppDomainResult> ItemGroupOption()
        {
            var data = await dropdownService.ItemGroupOption();
            return new AppDomainResult(data);
        }

        [HttpGet("feedback-group/{branchId}")]
        [AppAuthorize]
        [Description("Lấy danh sách nhóm phản hồi")]
        public async Task<AppDomainResult> FeedbackGroupOption(Guid branchId)
        {
            var data = await dropdownService.FeedbackGroupOption(branchId);
            return new AppDomainResult(data);
        }

        [HttpGet("lookup/{code}")]
        [AppAuthorize]
        [Description("Lấy danh sách loại phiếu nhập / xuất kho")]
        public async Task<AppDomainResult> LookupOption(string code)
        {
            var data = await dropdownService.LookupOption(code);
            return new AppDomainResult(data);
        }

        [HttpGet("vendor")]
        [AppAuthorize]
        [Description("Lấy danh sách nhà cung cấp")]
        public async Task<AppDomainResult> VendorOption()
        {
            var data = await dropdownService.VendorOption();
            return new AppDomainResult(data);
        }

        [HttpGet("item-sku/{branchId}")]
        [AppAuthorize]
        [Description("Lấy danh sách nguyên liệu trong kho")]
        public async Task<AppDomainResult> ItemSKUOption(Guid branchId)
        {
            var data = await dropdownService.ItemSKUOption(branchId);
            return new AppDomainResult(data);
        }

        [HttpGet("item/{branchId}")]
        [AppAuthorize]
        [Description("Lấy danh sách nguyên liệu trong kho")]
        public async Task<AppDomainResult> ItemOption(Guid branchId)
        {
            var data = await dropdownService.ItemOption(branchId);
            return new AppDomainResult(data);
        }

        [HttpGet("parent/{branchId}")]
        [AppAuthorize]
        [Description("Lấy danh sách phụ huynh")]
        public async Task<AppDomainResult> ParentOption(Guid branchId)
        {
            var data = await dropdownService.ParentOption(branchId);
            return new AppDomainResult(data);
        }

        [HttpGet("food/{branchId}")]
        [AppAuthorize]
        [Description("Lấy danh sách món ăn")]
        public async Task<AppDomainResult> FoodOption(Guid branchId)
        {
            var data = await dropdownService.FoodOption(branchId);
            return new AppDomainResult(data);
        }

        [HttpGet("nutrition-group")]
        [AppAuthorize]
        [Description("Lấy danh sách nhóm dinh dưỡng")]
        public async Task<AppDomainResult> NutritionGroupOption([FromQuery] NutritionGroupOptionSearch request)
        {
            var data = await dropdownService.NutritionGroupOption(request);
            return new AppDomainResult(data);
        }

        [HttpGet("menu/{branchId}")]
        [AppAuthorize]
        [Description("Lấy danh sách món ăn")]
        public async Task<AppDomainResult> MenuOption(Guid branchId)
        {
            var data = await dropdownService.MenuOption(branchId);
            return new AppDomainResult(data);
        }

        [HttpGet("purchase-order/{branchId}")]
        [AppAuthorize]
        [Description("Lấy danh sách phiếu đi chợ")]
        public async Task<AppDomainResult> PurchaseOrderOption(Guid branchId)
        {
            var data = await dropdownService.PurchaseOrderOption(branchId);
            return new AppDomainResult(data);
        }

        [HttpGet("fee-by-grade")]
        [AppAuthorize]
        [Description("Lấy danh sách khoản thu")]
        public async Task<AppDomainResult> FeeByGradeOption([FromQuery]FeeOptionSearch search)
        {
            var data = await dropdownService.FeeByGradeOption(search);
            return new AppDomainResult(data);
        }


        [HttpGet("fee/{branchId}")]
        [AppAuthorize]
        [Description("Lấy danh sách khoản thu")]
        public async Task<AppDomainResult> FeeOption(Guid branchId)
        {
            var data = await dropdownService.FeeOption(branchId);
            return new AppDomainResult(data);
        }

        [HttpGet("fee-reduction/{branchId}")]
        [AppAuthorize]
        [Description("Lấy danh sách miễn giảm")]
        public async Task<AppDomainResult> FeeReductionOption(Guid branchId)
        {
            var data = await dropdownService.FeeReductionOption(branchId);
            return new AppDomainResult(data);
        }

        [HttpGet("payment-bank")]
        [AppAuthorize]
        [Description("Lấy danh sách ngân hàng")]
        public async Task<AppDomainResult> PaymentBankOption()
        {
            var data = await dropdownService.PaymentBankOption();
            return new AppDomainResult(data);
        }

        [HttpGet("payment-method/{branchId}")]
        [AppAuthorize]
        [Description("Lấy danh sách phương thức thanh toán")]
        public async Task<AppDomainResult> PaymentMethodOption(Guid branchId)
        {
            var data = await dropdownService.PaymentMethodOption(branchId);
            return new AppDomainResult(data);
        }

        [HttpGet("collection-plan/{branchId}")]
        [AppAuthorize]
        [Description("Lấy danh sách phương kế hoạch thu")]
        public async Task<AppDomainResult> CollectionPlanOption(Guid branchId)
        {
            var data = await dropdownService.CollectionPlanOption(branchId);
            return new AppDomainResult(data);
        }
        [HttpGet("study-shift")]
        [AppAuthorize]
        [Description("Lấy danh sách ca học")]
        public async Task<AppDomainResult> StudyShiftOption()
        {
            var data = await dropdownService.StudyShiftOption();
            return new AppDomainResult(data);
        }
        [HttpGet("study-session/{branchId}")]
        [AppAuthorize]
        [Description("Lấy danh sách ca học")]
        public async Task<AppDomainResult> StudySessionOption(Guid branchId)
        {
            var data = await dropdownService.StudySessionOption(branchId);
            return new AppDomainResult(data);
        }
        [HttpGet("subject-in-class/{classId}")]
        [AppAuthorize]
        [Description("Lấy danh môn học của lớp")]
        public async Task<AppDomainResult> SubjectInClass(Guid classId)
        {
            var data = await dropdownService.SubjectInClassOption(classId);
            return new AppDomainResult(data);
        }
        [HttpGet("teacher-not-assignment")]
        [AppAuthorize]
        [Description("Lấy danh sách giáo viên chưa được phân công chủ nhiệm")]
        public async Task<AppDomainResult> TeacherNotAssignmentOption([FromQuery] TeacherNotAssignmentOptionSearch search)
        {
            var data = await dropdownService.TeacherNotAssignmentOption(search);
            return new AppDomainResult(data);
        }
    }
}
