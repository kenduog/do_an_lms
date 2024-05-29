using OfficeOpenXml;
using System;
using System.Collections.Generic;

namespace API.Model
{
    public class TeacherAvailableWhenBooking
    {
        public Guid teacherId { get; set; }
        public string teacherCode { get; set; }
        public string teacherName { get; set; }
        /// <summary>
        /// Ca học
        /// </summary>
        public List<ShiftAvailable> shifts { get; set; }
        public TeacherAvailableWhenBooking()
        {
            shifts = new List<ShiftAvailable>();
        }
    }
    public class ShiftAvailable
    {
        public double? sTime { get; set; }
        public double? eTime { get; set; }
        /// <summary>
        /// 1 - Đang mở
        /// 2 - Đã đặt
        /// </summary>
        public int status { get; set; }
        public string statusName { get { return status == 1 ? "Đang mở" : status == 2 ? "Đã đặt" : ""; } }
    }
}
