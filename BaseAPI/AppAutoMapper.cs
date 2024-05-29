using Entities;
using Models;
using Request;
using Utilities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Request.DomainRequests;
using Request.RequestCreate;
using Request.RequestUpdate;
using Entities.Search;
using Entities.DomainEntities;
using Entities.AuthEntities;
using Entities.DataTransferObject;

namespace Models.AutoMapper
{
    public class AppAutoMapper : Profile
    {
        public AppAutoMapper()
        {
            //Loại khoản thu
            CreateMap<TuitionConfigCategoryCreate, tbl_TuitionConfigCategory>().ReverseMap();
            CreateMap<TuitionConfigCategoryUpdate, tbl_TuitionConfigCategory>().ReverseMap();

            CreateMap<ReportTemplateCreate, tbl_ReportTemplate>().ReverseMap();
            CreateMap<ReportTemplateUpdate, tbl_ReportTemplate>().ReverseMap();

            //Tiết học
            CreateMap<PaymentBankCreate, tbl_PaymentBank>().ReverseMap();
            CreateMap<PaymentBankUpdate, tbl_PaymentBank>().ReverseMap();
            CreateMap<PaymentMethodCreate, tbl_PaymentMethod>().ReverseMap();
            CreateMap<PaymentMethodUpdate, tbl_PaymentMethod>().ReverseMap();

            //Tiết học
            CreateMap<StudySessionCreate, tbl_StudySession>().ReverseMap();
            CreateMap<StudySessionUpdate, tbl_StudySession>().ReverseMap();

            //Kế hoạch thu tiền
            CreateMap<CollectionSessionHeaderUpdate, tbl_CollectionSessionHeader>().ReverseMap();
            CreateMap<tbl_CollectionSessionHeader, CollectionSessionItem> ()
                .ForMember(x=>x.fees, s=> s.Ignore())
                .ForMember(x=>x.reductionFees, s=> s.Ignore())
                .ForMember(x=>x.amt, s=> s.Ignore())
                .ReverseMap();

            CreateMap<CollectionSessionCreate, tbl_CollectionSession>().ReverseMap();
            CreateMap<CollectionSessionUpdate, tbl_CollectionSession>().ReverseMap();

            //Kế hoạch thu tiền
            CreateMap<CollectionPlanDetailCreate, tbl_CollectionPlanDetail>().ReverseMap();
            CreateMap<CollectionPlanDetailUpdate, tbl_CollectionPlanDetail>().ReverseMap();
            CreateMap<CollectionPlanCreate, tbl_CollectionPlan>().ReverseMap();
            CreateMap<CollectionPlanUpdate, tbl_CollectionPlan>().ReverseMap();

            CreateMap<MenuWeekItemUpdate, tbl_MenuWeek>().ReverseMap();

            //Miễn giảm học phí
            CreateMap<FeeReductionUpdate, tbl_FeeReduction>().ReverseMap();
            CreateMap<FeeReductionCreate, tbl_FeeReduction>().ReverseMap();
            CreateMap<FeeReductionConfigCreate, tbl_FeeReductionConfig>().ReverseMap();
            CreateMap<FeeReductionConfigUpdate, tbl_FeeReductionConfig>().ReverseMap();

            //Khoản cần thu
            CreateMap<FeeUpdate, tbl_Fee>().ReverseMap();
            CreateMap<FeeCreate, tbl_Fee>().ReverseMap();
            CreateMap<FeeInGradeCreate, tbl_FeeInGrade>().ReverseMap();
            CreateMap<FeeInGradeUpdate, tbl_FeeInGrade>().ReverseMap();

            //phieu di cho
            CreateMap<PurchaseOrderHeaderUpdate, tbl_PurchaseOrderHeader>().ReverseMap();
            CreateMap<PurchaseOrderHeaderCreate, tbl_PurchaseOrderHeader>().ReverseMap();
            CreateMap<PurchaseOrderLineCreate, tbl_PurchaseOrderLine>().ReverseMap();
            CreateMap<PurchaseOrderLineUpdate, tbl_PurchaseOrderLine>().ReverseMap();

            //nhóm dinh dưỡng
            CreateMap<NutritionGroupCreate, tbl_NutritionGroup>().ReverseMap();
            CreateMap<NutritionGroupUpdate, tbl_NutritionGroup>().ReverseMap();

            //menu food
            CreateMap<MenuFoodCreate, tbl_MenuFood>().ReverseMap();
            CreateMap<MenuFoodUpdate, tbl_MenuFood>().ReverseMap();

            //menu item
            CreateMap<MenuItemCreate, tbl_MenuItem>().ReverseMap();
            CreateMap<MenuItemUpdate, tbl_MenuItem>().ReverseMap();

            //Menu
            CreateMap<MenuCreate, tbl_Menu>().ReverseMap();
            CreateMap<MenuUpdate, tbl_Menu>().ReverseMap();

            //Món ăn
            CreateMap<FoodItemCreate, tbl_FoodItem>().ReverseMap();
            CreateMap<FoodItemUpdate, tbl_FoodItem>().ReverseMap();

            //Món ăn
            CreateMap<FoodCreate, tbl_Food>().ReverseMap();
            CreateMap<FoodUpdate, tbl_Food>().ReverseMap();

            CreateMap<tbl_ItemInventory, ItemInventoryExport>().ReverseMap();

            CreateMap<tbl_Staff, ItemStaffExport>().ReverseMap();
            CreateMap<tbl_Staff, ItemStaffImport>().ReverseMap();
            CreateMap<tbl_Staff, ItemTeacherExport>().ReverseMap();

            CreateMap<tbl_ValueEntry, ValueEntryExport>()
                .ForMember(x => x.date, d => d.MapFrom(s => Timestamp.ToString(s.date, "dd/MM/yyyy")));

            //Nha cung cap
            CreateMap<VendorCreate, tbl_Vendor>().ReverseMap();
            CreateMap<VendorUpdate, tbl_Vendor>().ReverseMap();

            //Xuat kho
            CreateMap<DeliveryOrderHeaderUpdate, tbl_DeliveryOrderHeader>().ReverseMap();
            CreateMap<DeliveryOrderHeaderCreate, tbl_DeliveryOrderHeader>().ReverseMap();
            CreateMap<DeliveryOrderLineCreate, tbl_DeliveryOrderLine>().ReverseMap();
            CreateMap<DeliveryOrderLineUpdate, tbl_DeliveryOrderLine>().ReverseMap();

            //Nhap kho
            CreateMap<ReceiveOrderHeaderUpdate, tbl_ReceiveOrderHeader>().ReverseMap();
            CreateMap<ReceiveOrderHeaderCreate, tbl_ReceiveOrderHeader>().ReverseMap();
            CreateMap<ReceiveOrderLineCreate, tbl_ReceiveOrderLine>().ReverseMap();
            CreateMap<ReceiveOrderLineUpdate, tbl_ReceiveOrderLine>().ReverseMap();

            CreateMap<tbl_ScaleMeasureDetail, ScaleMeasureDetailExport>()
                .ForMember(x => x.studentBirthDay, d => d.MapFrom(s => Timestamp.ToString(s.studentBirthDay, "dd/MM/yyyy")))
                .ForMember(x => x.studentGenderName, d => d.MapFrom(s => s.studentGender == 1 ? "Nam" : s.studentGender == 2 ? "Nữ" : "Khác"));

            CreateMap<tbl_Schedule, tbl_TimeTableDetail>().ReverseMap();

            //cấu hình feedback permission
            CreateMap<FeedbackPermissionUpdate, tbl_FeedbackPermission>().ReverseMap();

            //Nhóm phản hồi
            CreateMap<FeedbackPermissionCreate, tbl_FeedbackPermission>().ReverseMap();
            CreateMap<FeedbackPermissionUpdate, tbl_FeedbackPermission>().ReverseMap();

            //Nhóm phản hồi
            CreateMap<FeedbackGroupCreate, tbl_FeedbackGroup>().ReverseMap();
            CreateMap<FeedbackGroupUpdate, tbl_FeedbackGroup>().ReverseMap();

            //Phản hồi
            CreateMap<FeedbackCreate, tbl_Feedback>().ReverseMap();
            CreateMap<FeedbackUpdate, tbl_Feedback>().ReverseMap();
            CreateMap<FeedbackVote, tbl_Feedback>().ReverseMap();

            //Nhận xét phản hồi
            CreateMap<FeedbackReplyCreate, tbl_FeedbackReply>().ReverseMap();
            CreateMap<FeedbackReplyUpdate, tbl_FeedbackReply>().ReverseMap();

            //SKU item
            CreateMap<ItemOfSKUCreate, tbl_ItemOfSKU>().ReverseMap();
            CreateMap<ItemOfSKUUpdate, tbl_ItemOfSKU>().ReverseMap();

            //don vi tinh
            CreateMap<UnitOfMeasureCreate, tbl_UnitOfMeasure>().ReverseMap();
            CreateMap<UnitOfMeasureUpdate, tbl_UnitOfMeasure>().ReverseMap();

            //nhom nguyen lieu
            CreateMap<ItemGroupCreate, tbl_ItemGroup>().ReverseMap();
            CreateMap<ItemGroupUpdate, tbl_ItemGroup>().ReverseMap();

            //nguyen lieu ban tru
            CreateMap<ItemCreate, tbl_Item>().ReverseMap();
            CreateMap<ItemUpdate, tbl_Item>().ReverseMap();


            //Lịch học
            CreateMap<ScheduleCreate, tbl_Schedule>().ReverseMap();
            CreateMap<ScheduleUpdate, tbl_Schedule>().ReverseMap();

            //Chi tiết thời khóa biểu
            CreateMap<TimeTableDetailCreate, tbl_TimeTableDetail>().ReverseMap();
            CreateMap<TimeTableDetailUpdate, tbl_TimeTableDetail>().ReverseMap();

            //phân phối chương trình cho khối
            CreateMap<ItemSubjectInGrade, tbl_SubjectInGrade>().ReverseMap();
            CreateMap<SubjectInGradeUpdate, tbl_SubjectInGrade>().ReverseMap();

            //xin vắng
            CreateMap<SubjectGroupCreate, tbl_SubjectGroup>().ReverseMap();
            CreateMap<SubjectGroupUpdate, tbl_SubjectGroup>().ReverseMap();


            //xin vắng
            CreateMap<StudentLeaveRequestCreate, tbl_StudentLeaveRequest>().ReverseMap();
            CreateMap<StudentLeaveRequestUpdate, tbl_StudentLeaveRequest>().ReverseMap();

            //điểm danh
            CreateMap<AttendanceCreate, tbl_Attendance>().ReverseMap();
            CreateMap<AttendanceUpdate, tbl_Attendance>().ReverseMap();

            //Phòng
            CreateMap<RoomCreate, tbl_Room>().ReverseMap();
            CreateMap<RoomUpdate, tbl_Room>().ReverseMap();
            //Khuyến mãi
            CreateMap<DiscountCreate, tbl_Discount>().ReverseMap();
            CreateMap<DiscountUpdate, tbl_Discount>().ReverseMap();
            //Công nợ
            CreateMap<BillCreate, tbl_Bill>().ReverseMap();
            CreateMap<BillUpdate, tbl_Bill>().ReverseMap();
            //Thu chi
            CreateMap<PaymentSessionCreate, tbl_PaymentSession>().ReverseMap();
            CreateMap<PaymentSessionUpdate, tbl_PaymentSession>().ReverseMap();
            //Ca học
            CreateMap<StudyShiftCreate, tbl_StudyShift>().ReverseMap();
            CreateMap<StudyShiftUpdate, tbl_StudyShift>().ReverseMap();

            //phụ huynh
            CreateMap<ParentCreate, tbl_Parent>().ReverseMap();
            CreateMap<ParentUpdate, tbl_Parent>().ReverseMap();
            CreateMap<ParentCreate, tbl_Users>().ReverseMap();
            CreateMap<ParentUpdate, tbl_Users>().ReverseMap();

            //Cấu hình học phí
            CreateMap<TuitionConfigCreate, tbl_TuitionConfig>().ReverseMap();
            CreateMap<TuitionConfigUpdate, tbl_TuitionConfig>().ReverseMap();

            //trình độ học vấn cao nhất
            CreateMap<HighestLevelOfEducationCreate, tbl_HighestLevelOfEducation>().ReverseMap();
            CreateMap<HighestLevelOfEducationUpdate, tbl_HighestLevelOfEducation>().ReverseMap();

            //Khung nhân viên
            CreateMap<PositionCreate, tbl_Position>().ReverseMap();
            CreateMap<PositionUpdate, tbl_Position>().ReverseMap();

            //Khung nhân viên
            CreateMap<StaffCreate, tbl_Staff>().ReverseMap();
            CreateMap<StaffUpdate, tbl_Staff>().ReverseMap();
            CreateMap<StaffCreate, tbl_Users>().ReverseMap();
            CreateMap<StaffUpdate, tbl_Users>().ReverseMap();

            //Khung chương trình
            CreateMap<TeachingFrameCreate, tbl_TeachingFrame>().ReverseMap();
            CreateMap<TeachingFrameUpdate, tbl_TeachingFrame>().ReverseMap();

            //Môn học
            CreateMap<SubjectCreate, tbl_Subject>().ReverseMap();
            CreateMap<SubjectUpdate, tbl_Subject>().ReverseMap();

            //Quyền của nhóm
            CreateMap<PermissionCreate, tbl_Permission>().ReverseMap();
            CreateMap<PermissionUpdate, tbl_Permission>().ReverseMap();

            //Quyền của nhóm
            CreateMap<GroupPermissionCreate, tbl_GroupPermission>().ReverseMap();
            CreateMap<GroupPermissionUpdate, tbl_GroupPermission>().ReverseMap();

            //User trong nhóm
            CreateMap<UserGroupCreate, tbl_UserGroup>().ReverseMap();
            CreateMap<UserGroupUpdate, tbl_UserGroup>().ReverseMap();

            //Menu
            CreateMap<ContentTypeCreate, tbl_ContentType>().ReverseMap();
            CreateMap<ContentTypeUpdate, tbl_ContentType>().ReverseMap();

            //Nhóm quyền
            CreateMap<GroupCreate, tbl_Group>().ReverseMap();
            CreateMap<GroupUpdate, tbl_Group>().ReverseMap();

            //Học viên trong 
            CreateMap<StudentInClassCreate, tbl_StudentInClass>().ReverseMap();
            CreateMap<StudentInClassUpdate, tbl_StudentInClass>().ReverseMap();

            //Lớp học
            CreateMap<ClassCreate, tbl_Class>().ReverseMap();
            CreateMap<ClassUpdate, tbl_Class>().ReverseMap();

            //Học sinh
            CreateMap<StudentCreate, tbl_Student>().ReverseMap();
            CreateMap<StudentUpdate, tbl_Student>().ReverseMap();
            CreateMap<tbl_Student, ItemStudentExport>().ReverseMap();

            //Mẫu hồ sơ 
            CreateMap<ProfileTemplateCreate, tbl_ProfileTemplate>().ReverseMap();
            CreateMap<ProfileTemplateUpdate, tbl_ProfileTemplate>().ReverseMap();

            //Chi tiết bộ tiêu chuẩn
            CreateMap<HolidayCreate, tbl_Holiday>().ReverseMap();
            CreateMap<HolidayUpdate, tbl_Holiday>().ReverseMap();

            //Ngày học
            CreateMap<DayOfWeekUpdate, tbl_DayOfWeek>().ReverseMap();

            //Chi tiết bộ tiêu chuẩn
            CreateMap<CriteriaDetailCreate, tbl_CriteriaDetail>().ReverseMap();
            CreateMap<CriteriaDetailUpdate, tbl_CriteriaDetail>().ReverseMap();

            //Bộ tiêu chuẩn
            CreateMap<CriteriaCreate, tbl_Criteria>().ReverseMap();
            CreateMap<CriteriaUpdate, tbl_Criteria>().ReverseMap();

            //Cấp lớp
            CreateMap<GradeCreate, tbl_Grade>().ReverseMap();
            CreateMap<GradeUpdate, tbl_Grade>().ReverseMap();

            //Học kỳ
            CreateMap<SemesterCreate, tbl_Semester>().ReverseMap();
            CreateMap<SemesterUpdate, tbl_Semester>().ReverseMap();

            //Năm học
            CreateMap<SchoolYearCreate, tbl_SchoolYear>().ReverseMap();
            CreateMap<SchoolYearUpdate, tbl_SchoolYear>().ReverseMap();

            //người dùng
            CreateMap<UserCreate, tbl_Users>().ReverseMap();
            CreateMap<AdminCreate, tbl_Users>().ReverseMap();
            CreateMap<UserUpdate, tbl_Users>().ReverseMap();
            CreateMap<UserImport, tbl_Users>().ReverseMap();

            //Thông báo
            CreateMap<NotificationCreate, tbl_Notification>().ReverseMap();
            CreateMap<NotificationUpdate, tbl_Notification>().ReverseMap();
            
            //Chi nhánh
            CreateMap<BranchCreate, tbl_Branch>().ReverseMap();
            CreateMap<BranchUpdate, tbl_Branch>().ReverseMap();

            //Bài đăng
            CreateMap<NewsCreate, tbl_News>().ReverseMap();
            CreateMap<NewsUpdate, tbl_News>().ReverseMap();

            //Bình luận bài đăng
            CreateMap<CommentInNewsCreate, tbl_CommentInNews>().ReverseMap();
            CreateMap<CommentInNewsUpdate, tbl_CommentInNews>().ReverseMap();

            //Tài liệu bảng tin
            CreateMap<TypeDocumentNewsCreate, tbl_TypeDocumentNews>().ReverseMap();
            CreateMap<TypeDocumentNewsUpdate, tbl_TypeDocumentNews>().ReverseMap();

            //Nhóm bảng tin
            CreateMap<GroupNewsCreate, tbl_GroupNews>().ReverseMap();
            CreateMap<GroupNewsUpdate, tbl_GroupNews>().ReverseMap();

            //Phòng ban
            CreateMap<DepartmentCreate, tbl_Departments>().ReverseMap();
            CreateMap<DepartmentUpdate, tbl_Departments>().ReverseMap();

            //Tham gia nhóm bảng tin
            CreateMap<UserJoinGroupNewsCreate, tbl_UserJoinGroupNews>().ReverseMap();
            CreateMap<UserJoinGroupNewsMultipleCreate, tbl_UserJoinGroupNews>().ReverseMap();

            //Đăng file trong bảng tin
            CreateMap<DocumentNewsCreate, tbl_DocumentNews>().ReverseMap();
            CreateMap<DocumentNewsUpdate, tbl_DocumentNews>().ReverseMap();

            //Cấu hình nhận xét mặc định
            CreateMap<CommentDefaultCreate, tbl_CommentDefault>().ReverseMap();
            CreateMap<CommentDefaultUpdate, tbl_CommentDefault>().ReverseMap();

            //Nhận xét học sinh
            CreateMap<CommentCreate, tbl_Comment>().ReverseMap();
            CreateMap<CommentUpdate, tbl_Comment>().ReverseMap();
            
            //Phân công giảng dạy
            CreateMap<TeachingAssignmentCreate, tbl_TeachingAssignment>().ReverseMap();
            CreateMap<TeachingAssignmentUpdate, tbl_TeachingAssignment>().ReverseMap();
            
            //Thời khóa biểu
            CreateMap<TimeTableCreate, tbl_TimeTable>().ReverseMap();
            CreateMap<TimeTableUpdate, tbl_TimeTable>().ReverseMap();
            CreateMap<tbl_TimeTable, GenerateTimeTableDTO>().ReverseMap();

            //Phiếu bé ngoan
            CreateMap<GoodBehaviorCertificateUpdate, tbl_GoodBehaviorCertificate>().ReverseMap();

            //Cấu hình tuần trong học kỳ
            CreateMap<WeekCreate, tbl_Week>().ReverseMap();
            CreateMap<WeekUpdate, tbl_Week>().ReverseMap();

            //Cấu hình chủ đề đánh giá trẻ
            CreateMap<ChildAssessmentTopicCreate, tbl_ChildAssessmentTopic>().ReverseMap();
            CreateMap<ChildAssessmentTopicUpdate, tbl_ChildAssessmentTopic>().ReverseMap();

            //Cấu hình nội dung chủ đề đánh giá trẻ
            CreateMap<ChildAssessmentDetailCreate, tbl_ChildAssessmentDetail>().ReverseMap();
            CreateMap<ChildAssessmentDetailUpdate, tbl_ChildAssessmentDetail>().ReverseMap();

            //Cấu hình đợt cân đo
            CreateMap<ScaleMeasureCreate, tbl_ScaleMeasure>().ReverseMap();
            CreateMap<ScaleMeasureUpdate, tbl_ScaleMeasure>().ReverseMap();

            //Cấu hình chi tiết đợt cân đo
            CreateMap<ScaleMeasureDetailUpdate, tbl_ScaleMeasureDetail>().ReverseMap();
            CreateMap<ScaleMeasureDetailCreate, tbl_ScaleMeasureDetail>().ReverseMap();

            //Cấu hình thông báo
            CreateMap<NotificationConfigUpdate, tbl_NotificationConfig>().ReverseMap();
            CreateMap<NotificationConfigCreate, tbl_NotificationConfig>().ReverseMap();

            //Cấu hình bảng điểm
            CreateMap<TranscriptUpdate, tbl_Transcript>().ReverseMap();
        }
    }
}