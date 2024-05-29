using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Comment : DomainEntities.DomainEntities
    {
        /// <summary>
        /// Học sinh
        /// </summary>
        public Guid? studentId { get; set; }
        /// <summary>
        /// Lớp học
        /// </summary>
        public Guid? classId { get; set; }
        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? branchId { get; set; }
        /// <summary>
        /// Ngày nhận xét 
        /// </summary>
        public double? date { get; set; }
        /// <summary>
        /// Buổi ăn trưa
        /// </summary>
        public string lunch { get; set; }
        /// <summary>
        /// Buổi ăn chiều nhẹ
        /// </summary>
        public string afternoonSnack { get; set; }
        /// <summary>
        /// Buổi ăn chiều 
        /// </summary>
        public string afternoon { get; set; }
        /// <summary>
        /// Ngủ 
        /// </summary>
        public string sleep { get; set; }
        /// <summary>
        /// Nhận xét ngày 
        /// </summary>
        public string dayComment { get; set; }
        /// <summary>
        /// Vệ sinh 
        /// </summary>
        public string hygiene { get; set; }
        [NotMapped]
        public string studentCode { get; set; }
        [NotMapped]
        public string studentName { get; set; }
        [NotMapped]
        public string studentThumbnail { get; set; }
    }
}
