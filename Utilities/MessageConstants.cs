using AutoMapper.Internal.Mappers;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using static Utilities.CoreContants;

namespace Utilities
{
    public class MessageContants
    {
        public const string err = "err";
        public const string success = "success";
        public const string un_suport_file = "un_suport_file";
        public const string unauthorized = "unauthorized";
        public const string auth_expiried = "auth_expiried";
        public const string feature_for_super_user = "feature_for_super_user";

        #region field
        public const string req_paymentBankId = "req_paymentBankId";
        public const string req_year = "req_year";
        public const string req_month = "req_month";
        public const string req_collectionPlan = "req_collectionPlan";
        public const string req_currentDate = "req_currentDate";
        public const string req_title = "req_title";
        public const string req_feedbackGroupId = "req_feedbackGroupId";
        public const string req_feedbackId = "req_feedbackId";
        public const string req_day = "req_day";
        public const string req_period = "req_period";
        public const string req_remarkType = "req_remarkType";
        public const string req_acronym = "req_acronym";
        public const string req_staffId = "req_staffId";
        public const string req_branchId = "req_branchId";
        public const string req_startYear = "req_startYear";
        public const string req_isNewStudent = "req_isNewStudent";
        public const string req_permission = "req_permission";
        public const string req_group = "req_group";
        public const string req_user = "req_user";
        public const string req_name = "req_name";
        public const string req_semester = "req_semester";
        public const string req_firstName = "req_firstName";
        public const string req_lastName = "req_lastName";
        public const string req_code = "req_code";
        public const string req_username = "req_username";
        public const string req_fullName = "req_fullName";
        public const string req_min1 = "req_min1"; //yêu cầu tối thiểu 1 item
        public const string req_itemGroupId = "req_itemGroupId";
        public const string req_unitOfMeasureId = "req_unitOfMeasureId";
        public const string req_itemId = "req_itemId";
        public const string req_scaleMeasureId = "req_scaleMeasureId";
        public const string req_scaleMeasureDate = "req_scaleMeasureDate";
        public const string req_pos = "req_pos";
        public const string req_groupNews = "req_groupNews";
        public const string req_details = "req_details";
        public const string req_foodId = "req_foodId";
        public const string req_foods = "req_foods";
        public const string req_nutritionGroup = "req_nutritionGroup";
        public const string req_endDay = "req_endDay";
        public const string req_startDay = "req_startDay";
        public const string req_menuWeekId = "req_menuWeekId";

        #endregion

        #region select
        public const string req_collectionSessionId = "req_collectionSessionId";
        public const string req_collectionSessionHeader = "req_collectionSessionHeader";
        public const string req_feeId = "req_feeId";
        public const string req_AttendanceDate = "req_AttendanceDate";
        public const string req_contentType = "req_contentType";
        public const string req_classId = "req_classId";
        public const string req_studentId = "req_studentId";
        public const string req_type = "req_type";
        public const string req_cityId = "req_cityId";
        public const string req_districtId = "req_districtId";
        public const string req_wardId = "req_wardId";
        public const string req_criteriaId = "req_criteriaId";
        public const string req_schoolYearId = "req_schoolYearId";
        public const string req_semesterId = "req_semesterId";
        public const string req_gradeId = "req_gradeId";
        public const string req_subjectId = "req_subjectId";
        public const string req_stime = "req_stime";
        public const string req_etime = "req_etime";
        public const string req_teacherId = "req_teacherId";
        public const string req_subjectGroupId = "req_subjectGroupId";
        public const string req_timeTableId = "req_timeTableId";
        public const string req_weekId = "req_weekId";
        public const string req_notificationTitle = "req_notificationTitle";
        public const string req_notificationContent= "req_notificationContent";
        public const string req_selectStudent = "req_selectStudent";
        public const string req_classInfo = "req_classInfo";

        #endregion

        #region exsisted

        public const string exs_studySession = "exs_studySession";
        public const string exs_collectionSession = "exs_collectionSession";
        public const string exs_itemCode = "exs_itemCode";
        public const string exs_name = "exs_name";
        public const string exs_code = "exs_code";
        public const string exs_groupCode = "exs_groupCode";
        public const string exs_schoolYearCode = "exs_schoolYearCode";
        public const string exs_branchCode = "exs_branchCode";
        public const string exs_studentCode = "exs_studentCode";
        public const string exs_permissionCode = "exs_permissionCode";
        public const string exs_studyShift = "exs_studyShift";
        public const string exs_timetable_teacher = "exs_timetable_teacher";
        public const string exs_timetable_classroom = "exs_timetable_classroom";
        public const string exs_semester = "exs_semester";
        public const string exs_teacherShift = "exs_teacherShift";
        public const string exs_scheduleTeacher = "exs_scheduleTeacher";
        public const string exs_mainSKU = "exs_mainSKU";
        public const string exs_studentInScaleMeasure = "exs_studentInScaleMeasure";
        public const string exs_user_in_group_news = "exs_user_in_group_news";
        public const string exs_unitSKU = "exs_unitSKU";
        public const string exs_classTeacher = "exs_classTeacher";


        #endregion

        #region not exsisted
        public const string nf_teacherInBranch = "nf_teacherInBranch";
        public const string nf_reportTemplate = "nf_reportTemplate";
        public const string nf_CollectionSession = "nf_CollectionSession";
        public const string nf_collectionSessionLine = "nf_collectionSessionLine";
        public const string nf_collectionSessionHeader = "nf_collectionSessionHeader";
        public const string nf_feeReduction = "nf_feeReduction";
        public const string nf_fee = "nf_fee";
        public const string nf_purchaseOrderHeader = "nf_purchaseOrderHeader";
        public const string nf_nutritionGroup = "nf_nutritionGroup";
        public const string nf_menu = "nf_menu";
        public const string nf_food = "nf_food";
        public const string nf_studentInClass = "nf_studentInClass";
        public const string nf_parent = "nf_parent";
        public const string nf_news = "nf_news";
        public const string nf_feedbackGroup = "nf_feedbackGroup";
        public const string nf_feedback = "nf_feedback";
        public const string nf_scaleMeasure = "nf_scaleMeasure";
        public const string nf_timeTable = "nf_timeTable";
        public const string nf_subjectGroup = "nf_subjectGroup";
        public const string nf_department = "nf_department";
        public const string nf_highestLevelOfEducation = "nf_highestLevelOfEducation";
        public const string nf_position = "nf_position";
        public const string nf_ward = "nf_ward";
        public const string nf_city = "nf_city";
        public const string nf_district = "nf_district";
        public const string nf_permission = "nf_permission";
        public const string nf_group = "nf_group";
        public const string nf_contentType = "nf_contentType";
        public const string nf_role = "nf_role";
        public const string nf_class = "nf_class";
        public const string nf_grade = "nf_grade";
        public const string nf_subject = "nf_subject";
        public const string nf_semester = "nf_semester";
        public const string nf_student = "nf_student";
        public const string nf_profileTemplate = "nf_profileTemplate";
        public const string nf_file = "nf_file";
        public const string nf_schoolYear = "nf_schoolYear";
        public const string nf_paymentBank = "nf_paymentBank";
        public const string nf_branch = "nf_branch";
        public const string nf_criteria = "nf_criteria";
        public const string nf_item = "nf_item";
        public const string nf_teacher = "nf_teacher";
        public const string nf_week = "nf_week";
        public const string nf_unitOfMeasure = "nf_unitOfMeasure";
        public const string nf_itemGroup = "nf_itemGroup";
        public const string nf_groupNews = "nf_groupNews";
        public const string nf_others = "nf_others";
        public const string nf_userInGroupNews = "nf_userInGroupNews";
        public const string nf_vendor = "nf_vendor";
        public const string nf_deliveryOrderHeader = "nf_deliveryOrderHeader";
        public const string nf_collectionPlan = "nf_collectionPlan";
        public const string nf_receiveOrderHeader = "nf_receiveOrderHeader";
        public const string nf_studySession = "nf_studySession";
        public const string nf_dayOfWeek = "nf_dayOfWeek";
        public const string nf_holiday = "nf_holiday";


        #endregion

        #region range & compare
        public const string ml_200 = "ml_200";
        public const string cp_date = "cp_date";

        #endregion

        #region other
        public const string can_not_delete_sku_item = "can_not_delete_sku_item";
        public const string invalid_file = "invalid_file";
        public const string invalid_action = "invalid_action";
        public const string can_not_update_qty_of_main_sku = "can_not_update_qty_of_main_sku";
        public const string can_not_update_unit_of_main_sku = "can_not_update_unit_of_main_sku";
        public const string can_not_update_is_main_of_main_sku = "can_not_update_is_main_of_main_sku";
        public const string feedback_is_done = "feedback_is_done";
        public const string only_creator_can_vote_feedback = "only_creator_can_vote_feedback";
        public const string can_not_update_news_others = "can_not_update_news_others";
        public const string can_not_group_news_grant_permissions = "can_not_group_news_grant_permissions";
        public const string can_not_update_admin_for_owner_group_news = "can_not_update_admin_for_owner_group_news";
        public const string can_not_update_admin_for_parent_group_news = "can_not_update_admin_for_parent_group_news";
        public const string can_not_delete_owner_group_news = "can_not_delete_owner_group_news";
        public const string user_is_owner_group_news = "user_is_owner_group_news";
        public const string can_not_cancel_approved_bill = "can_not_cancel_approved_bill";
        public const string require_at_least_2_menu = "require_at_least_2_menu";
        public const string start_day_must_less_than_end_day = "start_day_must_less_than_end_day";
        public const string can_not_delete_collection_session_because_any_parent_paid = "can_not_delete_collection_session_because_any_parent_paid";
        public const string can_not_update_only_active_timetable = "can_not_update_only_active_timetable";
        public const string day_not_exist_in_week_config = "day_not_exist_in_week_config";
        public const string today_day_of_week_not_attendance = "today_day_of_week_not_attendance";
        public const string today_holiday_not_attendance = "today_holiday_not_attendance";
        public const string err_score = "err_score";
        public const string time_classshift_error = "time_classshift_error";
        public const string today_day_of_week_not_timetable = "today_day_of_week_not_timetable";
        public const string can_not_create_collection_status_lose = "can_not_create_collection_status_lose";

        #endregion
    }
}
