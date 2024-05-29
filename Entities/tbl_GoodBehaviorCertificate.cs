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
    public class tbl_GoodBehaviorCertificate : DomainEntities.DomainEntities
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
        /// Chi nhánh
        /// </summary>
        public Guid? weekId { get; set; }
        /// <summary>
        /// true: có phiếu bé ngoan
        /// false: không có phiếu
        /// </summary>
        public bool? status { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string note { get; set; }
        [NotMapped]
        public string studentCode { get; set; }
        [NotMapped]
        public string studentName { get; set; }
        [NotMapped]
        public string studentThumbnail { get; set; }
        /// <summary>
        /// Vắng phép
        /// </summary>
        [NotMapped]
        public int? authorizedAbsence { get; set; } = 0;
        /// <summary>
        /// Vắng không phép
        /// </summary>
        [NotMapped]
        public int? unauthorizedAbsence { get; set; } = 0;
        [NotMapped]
        public string branchName { get; set; }
        [NotMapped]
        public string weekName { get; set; }
        [NotMapped]
        public string className { get; set; }
        [NotMapped]
        public Guid fatherId { get; set; }
        [NotMapped]
        public Guid motherId { get; set; }
        [NotMapped]
        public Guid guardianId { get; set; }
    }

    public class LearningResultByWeek
    {
        /// <summary>
        /// Có phiếu bé ngoan hay không
        /// true: có phiếu bé ngoan
        /// false: không có phiếu
        /// </summary>
        public bool? status { get; set; }

        //thông tin điểm danh của 7 ngày học, kèm Id, truyền tiếp id này để lấy thông tin điểm danh và nhận xét của hôm đó
        public List<LearningResultItem> details { get; set; }
    }

    public class LearningResultItem
    {
        public Guid id { get; set; }
        /// <summary>
        /// 0 - Không học
        /// 1 - Có mặt
        /// 2 - Vắng phép
        /// 3 - Vắng không phép
        /// </summary>
        public int? status { get; set; } = 0;
        public double? date { get; set; }
    }
}
