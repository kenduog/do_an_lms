using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Schedule : DomainEntities.DomainEntities
    {
        public Guid? timeTableId { get; set; }
        public Guid? subjectId { get; set; }
        public Guid? teacherId { get; set; }
        public Guid? classId { get; set; }
        public Guid? weekId { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }

        /// <summary>
        /// 1 - Tiết trong tkb
        /// 2 - Tiết học bù
        /// </summary>
        public int? type { get; set; }

        [NotMapped]
        public string subjectName { get; set; }
        [NotMapped]
        public string teacherName { get; set; }
        [NotMapped]
        public int? subjectType { get; set; }
    }
}
