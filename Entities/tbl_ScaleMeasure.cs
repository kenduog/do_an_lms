using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_ScaleMeasure : DomainEntities.DomainEntities
    {
        public string name { get; set; }
        public double? date { get; set; }
        /// <summary>
        /// 1 = Toàn trường 
        /// 2 = Khối 
        /// 3 = Lớp 
        /// </summary>
        public int? type { get; set; }
        public Guid? branchId { get; set; }
        public Guid? schoolYearId { get; set; }
        public string gradeIds { get; set; }
        public string classIds { get; set; }
    }
}
