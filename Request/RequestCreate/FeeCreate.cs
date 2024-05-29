using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class FeeCreate : DomainCreate
    {
        [Required (ErrorMessage = MessageContants.req_branchId)]
        public Guid? branchId { get; set; }
        [Required (ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
        public string description { get; set; }
        /// <summary>
        /// Kỳ thu:
        /// 1 - Hàng tháng
        /// 2 - Quí
        /// 3 - Học kỳ
        /// 4 - Năm học
        /// </summary>
        public int? collectionType { get; set; }

        public List<FeeInGradeCreate> items { get; set; }
    }

    public class FeeInGradeCreate : DomainCreate
    {
        public Guid? gradeId { get; set; }
        public double? price { get; set; }
    }
}
