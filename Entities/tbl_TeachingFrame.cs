using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_TeachingFrame : DomainEntities.DomainEntities
    {
        public Guid? schoolYearId{ get; set; }
        public Guid? gradeId { get; set; }
        [NotMapped]
        public string gradeName { get; set; }
        public Guid? subjectId { get; set; }
        [NotMapped]
        public string subjectName { get; set; }
        /// <summary>
        /// Class per semester 
        /// </summary>
        public int? cps { get; set; }
        /// <summary>
        /// Class per week
        /// </summary>
        public int? cpw { get; set; }
    }

    public class TeachingFrameGroupByGrade : DomainEntities.DomainEntities
    {
        public Guid? schoolYearId { get; set; }
        public string code { get; set; }
        public string name { get; set; }

        public int? totalCps { get; set; }
        public int? totalCpw { get; set; }

        public int? totalRequired { get; set; }
        public int? totalActivity { get; set; }
        public int? totalNonRequired { get; set; }
    }
}
