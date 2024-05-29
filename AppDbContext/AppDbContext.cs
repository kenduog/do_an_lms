using Microsoft.EntityFrameworkCore;
using Entities;
using Extensions;
using Interface.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;
using Entities.AuthEntities;
using Microsoft.Extensions.Options;

namespace AppDbContext
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public AppDbContext() : base()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<TimeTableDetailResponse>();
            modelBuilder.Ignore<ClassToPrepare>();
            modelBuilder.Ignore<SubjectPrepare>();
            modelBuilder.Ignore<TeacherBySubjectReponse>();

            //modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TimeTableDetailResponse>().HasNoKey();
            modelBuilder.Entity<ClassToPrepare>().HasNoKey();
            modelBuilder.Entity<SubjectPrepare>().HasNoKey();
            modelBuilder.Entity<TeacherBySubjectReponse>().HasNoKey();
        }

        public DbSet<tbl_Template> tbl_Template { get; set; }
        public DbSet<tbl_TuitionConfigCategory> tbl_TuitionConfigCategory { get; set; }
        public DbSet<sys_Log> sys_Log { get; set; }
        public DbSet<tbl_ReportTemplate> tbl_ReportTemplate { get; set; }
        public DbSet<tbl_StudySession> tbl_StudySession { get; set; }
        public DbSet<tbl_PaymentBank> tbl_PaymentBank { get; set; }
        public DbSet<tbl_PaymentMethod> tbl_PaymentMethod { get; set; }
        public DbSet<tbl_CollectionSessionFee> tbl_CollectionSessionFee { get; set; }
        public DbSet<tbl_CollectionSessionHeader> tbl_CollectionSessionHeader { get; set; }
        public DbSet<tbl_CollectionSessionLine> tbl_CollectionSessionLine { get; set; }
        public DbSet<tbl_CollectionSession> tbl_CollectionSession { get; set; }
        public DbSet<tbl_CollectionPlanDetail> tbl_CollectionPlanDetail { get; set; }
        public DbSet<tbl_CollectionPlan> tbl_CollectionPlan { get; set; }
        public DbSet<tbl_FeeReductionConfig> tbl_FeeReductionConfig { get; set; }
        public DbSet<tbl_FeeReduction> tbl_FeeReduction { get; set; }
        public DbSet<tbl_FeeInGrade> tbl_FeeInGrade { get; set; }
        public DbSet<tbl_Fee> tbl_Fee { get; set; }
        public DbSet<tbl_PurchaseOrderLine> tbl_PurchaseOrderLine { get; set; }
        public DbSet<tbl_PurchaseOrderHeader> tbl_PurchaseOrderHeader { get; set; }
        public DbSet<tbl_NutritionGroup> tbl_NutritionGroup { get; set; }
        public DbSet<tbl_MenuWeek> tbl_MenuWeek { get; set; }
        public DbSet<tbl_MenuFood> tbl_MenuFood { get; set; }
        public DbSet<tbl_MenuItem> tbl_MenuItem { get; set; }
        public DbSet<tbl_Menu> tbl_Menu { get; set; }
        public DbSet<tbl_FoodItem> tbl_FoodItem { get; set; }
        public DbSet<tbl_Food> tbl_Food { get; set; }
        public DbSet<TeacherBySubjectReponse> TeacherBySubjectReponse { get; set; }
        public DbSet<SubjectPrepare> SubjectPrepare { get; set; }
        public DbSet<ClassToPrepare> ClassToPrepare { get; set; }

        public DbSet<tbl_Lookup> tbl_Lookup { get; set; }
        public DbSet<tbl_LookupType> tbl_LookupType { get; set; }
        public DbSet<tbl_ItemInventory> tbl_ItemInventory { get; set; }
        public DbSet<tbl_Vendor> tbl_Vendor { get; set; }
        public DbSet<tbl_ReceiveOrderHeader> tbl_ReceiveOrderHeader { get; set; }
        public DbSet<tbl_ReceiveOrderLine> tbl_ReceiveOrderLine { get; set; }
        public DbSet<tbl_DeliveryOrderHeader> tbl_DeliveryOrderHeader { get; set; }
        public DbSet<tbl_DeliveryOrderLine> tbl_DeliveryOrderLine { get; set; }
        public DbSet<tbl_Feedback> tbl_Feedback { get; set; }
        public DbSet<tbl_FeedbackGroup> tbl_FeedbackGroup { get; set; }
        public DbSet<tbl_FeedbackReply> tbl_FeedbackReply { get; set; }
        public DbSet<tbl_FeedbackPermission> tbl_FeedbackPermission{ get; set; }
        public DbSet<tbl_ValueEntry> tbl_ValueEntry { get; set; }
        public DbSet<tbl_UnitOfMeasure> tbl_UnitOfMeasure { get; set; }
        public DbSet<tbl_ItemGroup> tbl_ItemGroup { get; set; }
        public DbSet<tbl_ItemOfSKU> tbl_ItemOfSKU { get; set; }
        public DbSet<tbl_Item> tbl_Item { get; set; }
        public DbSet<tbl_Schedule> tbl_Schedule { get; set; }
        public DbSet<tbl_ClassShift> tbl_ClassShift { get; set; }
        public DbSet<tbl_TimeTableDetail> tbl_TimeTableDetail { get; set; }
        public DbSet<tbl_SubjectInGrade> tbl_SubjectInGrade { get; set; }
        public DbSet<tbl_SubjectGroup> tbl_SubjectGroup { get; set; }
        public DbSet<tbl_StudentLeaveRequest> tbl_StudentLeaveRequest { get; set; }
        public DbSet<tbl_Attendance> tbl_Attendance { get; set; }
        public DbSet<tbl_StudyShift> tbl_StudyShift { get; set; }
        public DbSet<tbl_PaymentSession> tbl_PaymentSession { get; set; }
        public DbSet<tbl_Discount> tbl_Discount { get; set; }
        public DbSet<tbl_BillDetail> tbl_BillDetail { get; set; }
        public DbSet<tbl_Bill> tbl_Bill { get; set; }
        public DbSet<tbl_TuitionConfig> tbl_TuitionConfig { get; set; }
        public DbSet<tbl_StudentInClass> tbl_StudentInClass { get; set; }
        public DbSet<tbl_HighestLevelOfEducation> tbl_HighestLevelOfEducation { get; set; }
        public DbSet<tbl_Position> tbl_Position { get; set; }
        public DbSet<tbl_TeachingFrame> tbl_TeachingFrame { get; set; }
        public DbSet<tbl_Subject> tbl_Subject { get; set; }
        public DbSet<tbl_Class> tbl_Class { get; set; }
        public DbSet<tbl_AutoGenCodeConfig> tbl_AutoGenCodeConfig { get; set; }
        public DbSet<tbl_ClassInTimeTable> tbl_ClassInTimeTable { get; set; }
        public DbSet<tbl_TimeTable> tbl_TimeTable { get; set; }
        public DbSet<tbl_UserPermission> tbl_UserPermission { get; set; }
        public DbSet<tbl_UserGroup> tbl_UserGroup { get; set; }
        public DbSet<tbl_Permission> tbl_Permission { get; set; }
        public DbSet<tbl_GroupPermission> tbl_GroupPermission { get; set; }
        public DbSet<tbl_Group> tbl_Group { get; set; }
        public DbSet<sys_MultipleMessage> sys_MultipleMessage { get; set; }
        public DbSet<tbl_ContentType> tbl_ContentType { get; set; }
        public DbSet<tbl_CriteriaDetail> tbl_CriteriaDetail { get; set; }
        public DbSet<tbl_Criteria> tbl_Criteria { get; set; }
        public DbSet<tbl_Grade> tbl_Grade { get; set; }
        public DbSet<tbl_Wards> tbl_Wards { get; set; }
        public DbSet<tbl_Necessary> tbl_Necessary { get; set; }
        public DbSet<tbl_Districts> tbl_Districts { get; set; }
        public DbSet<tbl_Cities> tbl_Cities { get; set; }
        public DbSet<tbl_Users> tbl_Users { get; set; }
        public DbSet<tbl_Role> tbl_Role { get; set; }
        public DbSet<tbl_Notification> tbl_Notification { get; set; }
        public DbSet<tbl_ClassLevel> tbl_ClassLevel { get; set; }
        public DbSet<tbl_Parent> tbl_Parent { get; set; }
        public DbSet<tbl_Profile> tbl_Profile { get; set; }
        public DbSet<tbl_ProfileTemplate> tbl_ProfileTemplate { get; set; }
        public DbSet<tbl_SchoolYear> tbl_SchoolYear { get; set; }
        public DbSet<tbl_Semester> tbl_Semester { get; set; }
        public DbSet<tbl_Staff> tbl_Staff { get; set; }
        public DbSet<tbl_Student> tbl_Student { get; set; }
        public DbSet<tbl_Branch> tbl_Branch { get; set; }
        public DbSet<tbl_Room> tbl_Room { get; set; }
        public DbSet<tbl_Holiday> tbl_Holiday { get; set; }
        public DbSet<tbl_DayOfWeek> tbl_DayOfWeek { get; set; }
        public DbSet<tbl_ExemptionOrReduction> tbl_ExemptionOrReduction { get; set; }
        public DbSet<tbl_GroupNews> tbl_GroupNews { get; set; }
        public DbSet<tbl_CommentInNews> tbl_CommentInNews { get; set; }
        public DbSet<tbl_DocumentNews> tbl_DocumentNews { get; set; }
        public DbSet<tbl_LikeInNews> tbl_LikeInNews { get; set; }
        public DbSet<tbl_News> tbl_News { get; set; }
        public DbSet<tbl_UserJoinGroupNews> tbl_UserJoinGroupNews { get; set; }
        public DbSet<tbl_TypeDocumentNews> tbl_TypeDocumentNews { get; set; }
        public DbSet<tbl_Departments> tbl_Departments { get; set; }
        public DbSet<tbl_CommentDefault> tbl_CommentDefaults { get; set; }
        public DbSet<tbl_Comment> tbl_Comments { get; set; }
        public DbSet<tbl_TeachingAssignment> tbl_TeachingAssignment { get; set; }
        public DbSet<tbl_Week> tbl_Weeks { get; set; }
        public DbSet<tbl_GoodBehaviorCertificate> tbl_GoodBehaviorCertificates { get; set; }
        public DbSet<tbl_ChildAssessmentTopic> tbl_ChildAssessmentTopics { get; set; }
        public DbSet<tbl_ChildAssessmentDetail> tbl_ChildAssessmentDetails { get; set; }
        public DbSet<tbl_StudentInAssessment> tbl_StudentInAssessments { get; set; }
        public DbSet<tbl_ScaleMeasure> tbl_ScaleMeasure { get; set; }
        public DbSet<tbl_ScaleMeasureDetail> tbl_ScaleMeasureDetail { get; set; }
        public DbSet<tbl_Transcript> tbl_Transcript { get; set; }

        public DbSet<tbl_NotificationConfig> tbl_NotificationConfigs { get; set; }
    }
}
