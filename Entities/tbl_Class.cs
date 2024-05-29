using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities
{
    public class tbl_Class : DomainEntities.DomainEntities
    {
        public string name { get; set; }
        public int? size { get; set; }
        public Guid? gradeId { get; set; }
        public Guid? schoolYearId { get; set; }
        public Guid? teacherId { get; set; }
        public Guid? branchId { get; set; }
        /// <summary>
        /// Số buổi / ngày (1 hoặc 2)
        /// </summary>
        public int? session { get; set; }
        /// <summary>
        /// 1 - Sáng
        /// 2 - Chiều
        /// 3 - Tối
        /// </summary>
        public int? sessionType { get; set; }
        [NotMapped]
        public string schoolYearName { get; set; }
        [NotMapped]
        public string gradeName { get; set; }
        [NotMapped]
        public string teacherName { get; set; }
        [NotMapped]
        public int countStudent { get; set; }
        [NotMapped]
        public string branchName { get; set; }
        [NotMapped]
        public List<ClassShift> classShifts { get; set; }
    }

    public class ClassToPrepare
    {
        public Guid? id { get; set; }
        public string name { get; set; }
        /// <summary>
        /// Số buổi / ngày (1 hoặc 2)
        /// </summary>
        public int? session { get; set; }
        /// <summary>
        /// 1 - Sáng
        /// 2 - Chiều
        /// 3 - Tối
        /// </summary>
        public int? sessionType { get; set; }

    }
    public class ClassShift
    {
        public int? day { get; set; }
        /// <summary>
        /// tiết
        /// </summary>
        public List<ClassShiftReriod> period { get; set; }
    }
    public class ClassShiftReriod
    {
        public Guid? id { get; set; }
        public int? period { get; set; }
        /// <summary>
        /// Giờ bắt đầu, tính tại 01/01/1970 GMT
        /// </summary>
        public double? sTime { get; set; }
        /// <summary>
        /// Giờ kết thúc, tính tại 01/01/1970 GMT
        /// </summary>
        public double? eTime { get; set; }
    }

}
