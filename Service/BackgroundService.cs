using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Entities;
using Entities.AuthEntities;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using static Utilities.CoreContants;

namespace Service
{
    public class BackgroundService
    {
        private static IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();
        private static readonly string connectionStrings = configuration.GetSection("ConnectionStrings:SampleDbContext").Value.ToString();
        private static DbContextOptions<AppDbContext.AppDbContext> options = new DbContextOptionsBuilder<AppDbContext.AppDbContext>()
                        .UseSqlServer(connectionStrings)
                        .Options;
        
        public static string AutoGenCode(AppDbContext.AppDbContext dbContext, string tableName)
        {
            var config = dbContext.tbl_AutoGenCodeConfig
                .FirstOrDefault(d => d.tableName == tableName);
            if (config == null) return string.Empty;
            var now = DateTime.Today;
            string newCode = string.Empty;
            newCode = $"{config.prefix}-";

            if (config.isDay.HasValue && config.isDay.Value)
                newCode += now.Day.ToString("00");
            if (config.isMonth.HasValue && config.isMonth.Value)
                newCode += now.Month.ToString("00");
            if (config.isYear.HasValue && config.isYear.Value)
                newCode += $"{now:yy}-";
            newCode += (config.currentCode + 1).ToString().PadLeft(config.autoNumberLength ?? 0, '0');

            //update config
            config.currentCode++;
            dbContext.SaveChanges();
            return newCode;
        }
        public static async void ClearBranchInUser(Guid branchId)
        {
            using (var dbContext = new AppDbContext.AppDbContext(options))
            {
                using (var tran = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // tài khoản người dùng
                        var users = dbContext.tbl_Users
                    .Where(x => x.branchIds.Contains(branchId.ToString()) && x.deleted == false).ToList();
                        if (users.Any())
                        {
                            foreach (var user in users)
                            {
                                var branchIds = user.branchIds.Split(',').Where(x => x != branchId.ToString()).ToList();
                                user.branchIds = string.Join(",", branchIds);
                                if (String.IsNullOrEmpty(user.branchIds))
                                {
                                    user.deleted = true;
                                    // thành viên nhóm bảng tin
                                    var userJoinGroupNews = dbContext.tbl_UserJoinGroupNews.Where(x => x.userId == user.id && x.deleted == false).ToList();
                                    userJoinGroupNews.ForEach(x => x.deleted = true);
                                }
                            }
                        }
                        // nhân viên + giáo viên
                        var staffs = dbContext.tbl_Staff.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (staffs.Any())
                        {
                            staffs.ForEach(x => x.deleted = true);
                        }
                        // phụ huynh
                        var parents = dbContext.tbl_Staff.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (parents.Any())
                        {
                            parents.ForEach(x => x.deleted = true);
                        }
                        // học sinh
                        var students = dbContext.tbl_Student.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (students.Any())
                        {
                            students.ForEach(x => x.deleted = true);
                        }
                        // phòng học
                        var rooms = dbContext.tbl_Room.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (rooms.Any())
                        {
                            rooms.ForEach(x => x.deleted = true);
                        }
                        // lớp
                        var classes = dbContext.tbl_Class.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (classes.Any())
                        {
                            foreach (var _class in classes)
                            {
                                _class.deleted = true;
                                //lịch học
                                var schedules = dbContext.tbl_Schedule.Where(x => x.classId == _class.id && x.deleted == false).ToList();
                                schedules.ForEach(x => x.deleted = true);
                                // học viên trong lớp
                                var studentInClass = dbContext.tbl_StudentInClass.Where(x => x.classId == _class.id && x.deleted == false).ToList();
                                studentInClass.ForEach(x => x.deleted = true);
                                // Cập nhật lại trạng thái
                                var listStudent = new List<tbl_Student>();
                                foreach (var item in studentInClass)
                                {
                                    var student = dbContext.tbl_Student
                                    .FirstOrDefault(x => x.id == item.studentId);
                                    if (student != null)
                                    {
                                        student.status = (int)StudentStatus.Moi;
                                        listStudent.Add(student);
                                    }
                                }
                            }
                        }
                        // thiết lập tkb
                        var timeTables = dbContext.tbl_TimeTable.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (timeTables.Any())
                        {
                            foreach (var timeTable in timeTables)
                            {
                                timeTable.deleted = true;
                                // Nội dung tkb
                                var timeTableDetail = dbContext.tbl_TimeTableDetail.Where(x => x.timeTableId == timeTable.id && x.deleted == false).ToList();
                                timeTableDetail.ForEach(x => x.deleted = true);
                            }
                        }
                        // ca học
                        var studySession = dbContext.tbl_StudySession.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (studySession.Any())
                        {
                            studySession.ForEach(x => x.deleted = true);
                        }
                        // bài viết bảng tin
                        var news = dbContext.tbl_News
                           .Where(x => x.branchIds.Contains(branchId.ToString()) && x.deleted == false).ToList();
                        if (news.Any())
                        {
                            foreach (var n in news)
                            {
                                var branchIds = n.branchIds.Split(',').Where(x => x != branchId.ToString()).ToList();
                                n.branchIds = string.Join(",", branchIds);
                                if (String.IsNullOrEmpty(n.branchIds))
                                    n.deleted = true;
                            }
                        }
                        // cân đo
                        var scaleMeasures = dbContext.tbl_ScaleMeasure.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (scaleMeasures.Any())
                        {
                            foreach (var scaleMeasure in scaleMeasures)
                            {
                                scaleMeasure.deleted = true;
                                // chi tiết cân đo
                                var scaleMeasureDetail = dbContext.tbl_ScaleMeasureDetail.Where(x => x.scaleMeasureId == scaleMeasure.id && x.deleted == false).ToList();
                                scaleMeasureDetail.ForEach(x => x.deleted = true);
                            }
                        }
                        // món ăn
                        var foods = dbContext.tbl_Food.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (foods.Any())
                        {
                            foreach (var food in foods)
                            {
                                food.deleted = true;
                                // thực đơn món ăn
                                var foodItem = dbContext.tbl_FoodItem.Where(x => x.foodId == food.id && x.deleted == false).ToList();
                                foodItem.ForEach(x => x.deleted = true);
                            }
                        }
                        // thực phẩm
                        var items = dbContext.tbl_Item.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (items.Any())
                        {
                            items.ForEach(x => x.deleted = true);
                        }
                        // tồn kho
                        var itemInventories = dbContext.tbl_ItemInventory.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (itemInventories.Any())
                        {
                            itemInventories.ForEach(x => x.deleted = true);
                        }
                        // nhập kho
                        var receiveOrderHeaders = dbContext.tbl_ReceiveOrderHeader.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (receiveOrderHeaders.Any())
                        {
                            receiveOrderHeaders.ForEach(x => x.deleted = true);
                        }
                        // xuất kho
                        var deliveryOrderHeaders = dbContext.tbl_DeliveryOrderHeader.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (deliveryOrderHeaders.Any())
                        {
                            deliveryOrderHeaders.ForEach(x => x.deleted = true);
                        }
                        // thực đơn
                        var menus = dbContext.tbl_Menu.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (menus.Any())
                        {
                            foreach (var menu in menus)
                            {
                                menu.deleted = true;
                                // Chi tiết thực đơn
                                var menuDetail = dbContext.tbl_MenuItem.Where(x => x.menuId == menu.id && x.deleted == false).ToList();
                                menuDetail.ForEach(x => x.deleted = true);
                                // thực đơn tuần
                                var menuWeeks = dbContext.tbl_MenuWeek.Where(x => x.menuId == menu.id && x.deleted == false).ToList();
                                menuWeeks.ForEach(x => x.deleted = true);
                            }
                        }
                        // phiếu đi chợ
                        var purchaseOrderHeader = dbContext.tbl_PurchaseOrderHeader.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (purchaseOrderHeader.Any())
                        {
                            purchaseOrderHeader.ForEach(x => x.deleted = true);
                        }
                        // khối 
                        var grades = dbContext.tbl_Grade.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (grades.Any())
                        {
                            foreach (var g in grades)
                            {
                                ClearGrade(g.id);
                                g.deleted = true;
                            }
                        }
                        // nhóm môn hoc
                        var subjectGroup = dbContext.tbl_SubjectGroup.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (subjectGroup.Any())
                        {
                            subjectGroup.ForEach(x => x.deleted = true);
                        }
                        // giáo viên bộ môn
                        var teachingAssignment = dbContext.tbl_TeachingAssignment.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (teachingAssignment.Any())
                        {
                            teachingAssignment.ForEach(x => x.deleted = true);
                        }
                        // phân phối chương trình
                        var subjectInGrade = dbContext.tbl_SubjectInGrade.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (subjectInGrade.Any())
                        {
                            subjectInGrade.ForEach(x => x.deleted = true);
                        }
                        // nhận xét mặc định
                        var commentDefaults = dbContext.tbl_CommentDefaults.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (commentDefaults.Any())
                        {
                            commentDefaults.ForEach(x => x.deleted = true);
                        }
                        // chủ đề đánh giá
                        var childAssessmentTopics = dbContext.tbl_ChildAssessmentTopics.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (childAssessmentTopics.Any())
                        {
                            foreach (var topic in childAssessmentTopics)
                            {
                                topic.deleted = true;
                                // nội dung đánh giá
                                var childAssessmentDetails = dbContext.tbl_ChildAssessmentDetails.Where(x => x.childAssessmentId == topic.id && x.deleted == false).ToList();
                                childAssessmentDetails.ForEach(x => x.deleted = true);
                            }
                        }
                        // nhóm dinh dưỡng
                        var nutritionGroup = dbContext.tbl_NutritionGroup.Where(x => x.branchId == branchId && x.deleted == false).ToList();
                        if (nutritionGroup.Any())
                        {
                            nutritionGroup.ForEach(x => x.deleted = true);
                        }
                        tran.Commit();
                        await dbContext.SaveChangesAsync();
                    }
                    catch
                    {
                        tran.Rollback();
                        return;
                    }
                }
            }
        }
        public static async void ClearGrade(Guid id)
        {
            using (var dbContext = new AppDbContext.AppDbContext(options))
            {
                using (var tran = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // Kiểm tra nhóm đánh giá theo chủ đề
                        var childAssessmentTopic = dbContext.tbl_ChildAssessmentTopics
                            .Where(x => x.gradeId == id && x.deleted == false)
                            .ToList();
                        // Xóa topic
                        if (childAssessmentTopic.Count > 0)
                        {
                            foreach (var topic in childAssessmentTopic)
                            {
                                // Xóa topic chi tiết
                                var childAssessmentDetail = dbContext.tbl_ChildAssessmentDetails
                                .Where(x => x.childAssessmentId == topic.id && x.deleted == false)
                                .ToList();
                                childAssessmentDetail.ForEach(x => { x.deleted = true; });
                                topic.deleted = true;
                            }
                        }
                        // Khoản thu khối đó
                        var feeInGrade = dbContext.tbl_FeeInGrade
                        .Where(x => x.gradeId == id && x.deleted == false)
                        .ToList();
                        if (feeInGrade.Any())
                        {
                            feeInGrade.ForEach(x => x.deleted = true);
                        }
                        // Kế hoạch thu
                        var collectionPlan = dbContext.tbl_CollectionPlan
                        .Where(x => x.gradeId == id && x.deleted == false)
                        .ToList();
                        if (collectionPlan.Any())
                        {
                            collectionPlan.ForEach(x => x.deleted = true);
                        }
                        // Lớp học
                        var _class = dbContext.tbl_Class
                        .Where(x => x.gradeId == id && x.deleted == false)
                        .ToList();
                        if (_class.Any())
                        {
                            foreach (var c in _class)
                            {
                                var studentInClasses = dbContext.tbl_StudentInClass
                                .Where(x => x.classId == c.id && x.deleted == false).ToList();
                                var listStudent = new List<tbl_Student>();
                                foreach (var item in studentInClasses)
                                {
                                    var student = dbContext.tbl_Student
                                    .FirstOrDefault(x => x.id == item.studentId);
                                    if (student != null)
                                    {
                                        student.status = (int)StudentStatus.Moi;
                                        listStudent.Add(student);
                                    }
                                    item.deleted = true;
                                }
                                c.deleted = true;
                            }
                        }
                        // Thiết lập TKB
                        var timeTable = dbContext.tbl_TimeTable
                        .Where(x => x.gradeId == id && x.deleted == false)
                        .ToList();
                        if (timeTable.Any())
                        {
                            timeTable.ForEach(x => x.deleted = true);
                        }
                        // Cân đo
                        var scaleMeasure = dbContext.tbl_ScaleMeasure
                        .Where(x => x.gradeIds.Contains(id.ToString()) && x.deleted == false).ToList();
                        if (scaleMeasure.Any())
                        {
                            foreach (var scale in scaleMeasure)
                            {
                                var gradeIds = scale.gradeIds.Split(',').Where(x => x != id.ToString()).ToList();
                                scale.gradeIds = string.Join(",", gradeIds);
                            }
                        }
                        // Chi tiết cân đo
                        var scaleMeasureDetail = dbContext.tbl_ScaleMeasureDetail
                        .Where(x => x.gradeId == id && x.deleted == false).ToList();
                        if (scaleMeasureDetail.Any())
                        {
                            scaleMeasureDetail.ForEach(x => x.deleted = true);
                        }
                        // Phân phối chương trình
                        var subjectInGrade = dbContext.tbl_SubjectInGrade
                        .Where(x => x.gradeId == id && x.deleted == false).ToList();
                        if (subjectInGrade.Any())
                        {
                            subjectInGrade.ForEach(x => x.deleted = true);
                        }
                        // Thực đơn
                        var menu = dbContext.tbl_Menu.Where(x => x.gradeId == id && x.deleted == false).ToList();
                        if (menu.Any())
                        {
                            menu.ForEach(x => x.deleted = true);
                        }
                        // Nhóm dinh dưỡng
                        var nuti = dbContext.tbl_NutritionGroup.Where(x => x.gradeIds.Contains(id.ToString())
                        && x.deleted == false
                        ).ToList();
                        if (nuti.Any())
                        {
                            foreach (var n in nuti)
                            {
                                var gradeIds = n.gradeIds.Split(',').Where(x => x != id.ToString()).ToList();
                                n.gradeIds = string.Join(",", gradeIds);
                                if (String.IsNullOrEmpty(n.gradeIds))
                                {
                                    n.deleted = true;
                                }
                            }
                        }
                        tran.Commit();
                        await dbContext.SaveChangesAsync();
                    }
                    catch
                    {
                        tran.Rollback();
                        return;
                    }
                }
            }
        }
        public static async void ClearWeek(Guid weekId)
        {
            using (var dbContext = new AppDbContext.AppDbContext(options))
            {
                using (var tran = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // TKB
                        var schedule = dbContext.tbl_Schedule.Where(x => x.weekId == weekId && x.deleted == false).ToList();
                        if (schedule.Any())
                        {
                            schedule.ForEach(x => x.deleted = true);
                        }
                        // Phiếu bé ngoan
                        var goodBehaviorCertificates = dbContext.tbl_GoodBehaviorCertificates.Where(x => x.weekId == weekId && x.deleted == false).ToList();
                        if (goodBehaviorCertificates.Any())
                        {
                            goodBehaviorCertificates.ForEach(x => x.deleted = true);
                        }
                        var menuWeek = dbContext.tbl_MenuWeek.Where(x => x.weekId == weekId && x.deleted == false).ToList();
                        if (menuWeek.Any())
                        {
                            menuWeek.ForEach(x => x.deleted = true);
                        }
                        tran.Commit();
                        await dbContext.SaveChangesAsync();
                    }
                    catch
                    {
                        tran.Rollback();
                        return;
                    }
                }
            }
        }
        public static async void ClearSemester(Guid semesterId)
        {
            using (var dbContext = new AppDbContext.AppDbContext(options))
            {
                using (var tran = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // Kế hoạch thu
                        var collectionPlan = dbContext.tbl_CollectionPlan
                        .Where(x => x.semesterId == semesterId && x.deleted == false)
                        .ToList();
                        if (collectionPlan.Any())
                        {
                            collectionPlan.ForEach(x => x.deleted = true);
                        }
                        // Thiết lập TKB
                        var timeTable = dbContext.tbl_TimeTable
                         .Where(x => x.semesterId == semesterId && x.deleted == false)
                         .ToList();
                        if (timeTable.Any())
                        {
                            timeTable.ForEach(x => x.deleted = true);
                        }
                        // Tuần
                        var week = dbContext.tbl_Weeks.Where(x => x.semesterId == semesterId && x.deleted == false).ToList();
                        if (week.Any())
                        {
                            foreach (var w in week)
                            {
                                // Clear data
                                ClearWeek(w.id);
                                w.deleted = true;
                            }
                        }
                        tran.Commit();
                        await dbContext.SaveChangesAsync();
                    }
                    catch
                    {
                        tran.Rollback();
                        return;
                    }
                }
            }
        }
        public static async void ClearSchoolYear(Guid schoolYearId)
        {
            using (var dbContext = new AppDbContext.AppDbContext(options))
            {
                using (var tran = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // Học kì
                        var semester = dbContext.tbl_Semester.Where(x => x.schoolYearId == schoolYearId && x.deleted == false).ToList();
                        if (semester.Any())
                        {
                            foreach (var s in semester)
                            {
                                // Clear data
                                ClearSemester(s.id);
                                s.deleted = true;
                            }
                        }
                        // Lớp học
                        var _class = dbContext.tbl_Class
                        .Where(x => x.schoolYearId == schoolYearId && x.deleted == false)
                        .ToList();
                        if (_class.Any())
                        {
                            foreach (var c in _class)
                            {
                                var studentInClasses = dbContext.tbl_StudentInClass
                                .Where(x => x.classId == c.id && x.deleted == false).ToList();
                                var listStudent = new List<tbl_Student>();
                                foreach (var item in studentInClasses)
                                {
                                    var student = dbContext.tbl_Student
                                    .FirstOrDefault(x => x.id == item.studentId);
                                    if (student != null)
                                    {
                                        student.status = (int)StudentStatus.Moi;
                                        listStudent.Add(student);
                                    }
                                    item.deleted = true;
                                }
                                c.deleted = true;
                            }
                        }
                        tran.Commit();
                        await dbContext.SaveChangesAsync();
                    }
                    catch
                    {
                        tran.Rollback();
                        return;
                    }
                }
            }
        }

    }
}
