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
    public class tbl_Attendance : DomainEntities.DomainEntities
    {
        public Guid? branchId { get; set; }
        public Guid? schoolYearId { get; set; }
        public double? date { get; set; }
        public Guid? classId { get; set; }
        public Guid? studentId { get; set; }
        [NotMapped]
        public string studentFullName { get; set; }
        [NotMapped]
        public string studentCode { get; set; }
        [NotMapped]
        public string studentThumbnail { get; set; }
        /// <summary>
        /// 1 - Có mặt
        /// 2 - Vắng phép
        /// 3 - Vắng không phép
        /// </summary>
        public int? status { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int? available { get; set; } = 0;
        [JsonIgnore]
        [NotMapped]
        public int? leaveWithRequest { get; set; } = 0;
        [JsonIgnore]
        [NotMapped]
        public int? leaveWithoutRequest { get; set; } = 0;
        [JsonIgnore]
        [NotMapped]
        public int? count { get; set; } = 0;
        [NotMapped]
        public tbl_StudentLeaveRequest leaveRequest { get; set; }
    }

    public class AttendanceResponse
    {
        public int? totalItem { get; set; } = 0;
        public int? available { get; set; } = 0;
        public int? leaveWithRequest { get; set; } = 0;
        public int? leaveWithoutRequest { get; set; } = 0;
        public List<tbl_Attendance> items { get; set; }
    }
}
