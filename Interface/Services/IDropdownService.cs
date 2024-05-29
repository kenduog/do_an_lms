using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface IDropdownService 
    {
        Task<List<WeekOption>> WeekOption(Guid? semesterId);
        Task<List<DomainOption>> CityOption();
        Task<List<DomainOption>> DistrictOption(Guid? cityId);
        Task<List<DomainOption>> WardOption(Guid? districtId);
        Task<List<GroupOption>> GroupOption(GroupOptionSearch search);
        Task<List<GroupOption>> GroupStaffOption();
        Task<List<GroupOption>> FeedbackGroupStaffOption();
        Task<List<BranchOption>> BranchOption();
        Task<List<DomainOption>> GradeOption(Guid branchId);
        Task<List<DomainOption>> ClassOption(Guid gradeId);
        Task<List<SelectedOption>> SchoolYearOption();
        Task<List<SemesterOption>> SemesterOption(Guid? schoolYearId);
        Task<List<DomainOption>> DepartmentOption();
        Task<List<DomainOption>> HighestLevelOfEducationOption();
        Task<List<double>> StudyShiftFilterOption();
        Task<List<UserOption>> StaffOptionByCode(StaffOptionSearch search);
        Task<List<DiscountOption>> DiscountOption(Guid tuitionConfigId);
        Task<List<UserOption>> StudentOption(StudentOptionSearch search);
        Task<List<DomainOption>> SubjectGroupOption(Guid branchId);
        Task<List<DomainOption>> CriteriaOption(Guid gradeId);
        Task<List<DomainOption>> RoomOption(Guid branchId);
        Task<List<DomainOption>> UnitOfMeasureOption();
        Task<List<DomainOption>> ItemGroupOption();
        Task<List<DomainOption>> FeedbackGroupOption(Guid branchId);
        Task<List<SubjectOption>> SubjectOption(OptionSearch search);
        Task<List<LookupOption>> LookupOption(string typeCode);
        Task<List<DomainOption>> VendorOption();
        Task<List<ItemOption>> ItemSKUOption(Guid branchId);
        Task<List<ItemOption>> ItemOption(Guid branchId);
        Task<List<ParentOption>> ParentOption(Guid branchId);
        Task<List<FoodOption>> FoodOption(Guid branchId);
        Task<List<DomainOption>> NutritionGroupOption(NutritionGroupOptionSearch request);
        Task<List<MenuOption>> MenuOption(Guid branchId);
        Task<List<PurchaseOrderOption>> PurchaseOrderOption(Guid branchId);
        Task<List<DomainOption>> FeeOption(Guid branchId);
        Task<List<DomainOption>> FeeReductionOption(Guid branchId);
        Task<List<FeeOption>> FeeByGradeOption(FeeOptionSearch search);
        Task<List<PaymentBankOption>> PaymentBankOption();
        Task<List<PaymentMethodOption>> PaymentMethodOption(Guid branchId);
        Task<List<DomainOption>> CollectionPlanOption(Guid branchId);
        Task<List<StudyShiftOption>> StudyShiftOption();
        Task<List<StudySessionOption>> StudySessionOption(Guid branchId);
        Task<List<DomainOption>> SubjectInClassOption(Guid classId);
        Task<List<TeacherOption>> TeacherNotAssignmentOption(TeacherNotAssignmentOptionSearch search);
        Task<List<DomainOption>> TuitionConfigCategoryOption();
        Task<List<TuitionConfigOption>> TuitionConfigOption();
    }
}
