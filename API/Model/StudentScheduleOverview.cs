using System;
using System.Data.Entity.Migrations.Builders;

namespace API.Model
{
    public class StudentScheduleOverview
    {
        /// <summary>
        /// Trình độ đầu vào
        /// </summary>
        public string initialLevelName { get; set; }
        /// <summary>
        /// Trình độ hiện tại
        /// </summary>
        public string currentLevelName { get; set; }
        /// <summary>
        /// Đã đặt
        /// </summary>
        public int bookeds { get; set; }
        /// <summary>
        /// Hoàn thành
        /// </summary>
        public int asSchedules { get; set; }
        /// <summary>
        /// Học viên không đến
        /// </summary>
        public int studentNoShows { get; set; }
        /// <summary>
        /// Giáo viên không đến
        /// </summary>
        public int teacherNoShows { get; set; }
        /// <summary>
        /// Lỗi hệ thống
        /// </summary>
        public int itProblems { get; set; }
        /// <summary>
        /// Giáo viên đến trễ
        /// </summary>
        public int teacherLates { get; set; }
        /// <summary>
        /// Giáo viên hủy lịch
        /// </summary>
        public int teacherCancelations { get; set; }
        /// <summary>
        /// Học viên hủy lịch
        /// </summary>
        public int studentCancelations { get; set; }
        /// <summary>
        /// Tổng buổi học
        /// </summary>
        public int lessonAvailable { get; set; }
        /// <summary>
        /// Thời gian bắt đầu học
        /// </summary>
        public double? startDate { get; set; }
        /// <summary>
        /// Thời gian kết thúc học
        /// </summary>
        public double? endDate { get; set; }
    }
}
