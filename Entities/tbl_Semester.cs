using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Semester : DomainEntities.DomainEntities
    {
        /// <summary>
        /// Năm học
        /// </summary>
        public Guid? schoolYearId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        /// <summary>
        /// 1 = Học kì 1
        /// 2 = Học kì 2
        /// </summary>
        public int? semester { get; set; }
        /// <summary>
        /// Số tuần của học kỳ (mặc định học kỳ 1 là 18 tuần, học kỳ 2 là 17 tuần)
        /// </summary>
        public int? weeks { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
    }
}
