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
    public class tbl_Transcript : DomainEntities.DomainEntities
    {
        public Guid? schoolYearId { get; set; }
        public Guid? semesterId { get; set; }
        public Guid? classId { get; set; }
        public Guid? studentId { get; set; }
        public Guid? subjectId { get; set; }
        public int? score { get; set; }
        /// <summary>
        /// 1: thi giữa kỳ
        /// 2: thi học kỳ
        /// </summary>
        public int? type { get; set; }
        public string typeName { get; set; }
        public string note { get; set; }
        [NotMapped]
        public string className { get; set; }
        [NotMapped]
        public string studentCode { get; set; }
        [NotMapped]
        public string studentName { get; set; }
        [NotMapped]
        public string studentThumbnail { get; set; }
        [NotMapped]
        public string subjectName { get; set; }
    }
}
