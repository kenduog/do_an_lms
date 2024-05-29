using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_StudentInClass : DomainEntities.DomainEntities
    {
        public Guid? studentId { get; set; }
        public Guid? classId { get; set; }
        public Guid? gradeId { get; set; }
        public Guid? schoolYearId { get; set; }

        /// <summary>
        /// Trạng thái học sinh trong lớp
        /// 1 - Đang học
        /// 2 - Đã tốt nghiệp
        /// 3 - Thôi học
        /// 4 - Bảo lưu
        /// 5 - Học lại
        /// </summary>
        public int? status { get; set; }
        public string statusName { get; set; }
        public static string GetStatusName(int status) =>
            status == 1 ? "Đang học"
            : status == 2 ? "Đã tốt nghiệp"
            : status == 3 ? "Thôi học"
            : status == 4 ? "Bảo lưu" 
            : status == 5 ? "Học lại" : "";
        public string note { get; set; }
        [NotMapped]
        public string className { get; set; }
        [NotMapped]
        public string fullName { get; set; }
        [NotMapped]
        public string code { get; set; }
        [NotMapped]
        public string thumbnail { get; set; }
        [NotMapped]
        public int? gender { get; set; }
        [NotMapped]
        public string genderName { get; set; }
        [NotMapped]
        public int? type { get; set; }
        [NotMapped]
        public string typeName { get; set; }
        [NotMapped]
        public string nickname { get; set; }
        [NotMapped]
        public double? birthday  { get; set; }
        [NotMapped]
        public string schoolYearName { get; set; }
        [NotMapped]
        public string gradeName { get; set; }
    }
}
