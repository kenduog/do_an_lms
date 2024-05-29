using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestUpdate
{
    public class FeeUpdate : DomainUpdate
    {
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
        public List<FeeInGradeUpdate> items { get; set; }
    }

    public class FeeInGradeUpdate : DomainUpdate
    {
        public Guid? gradeId { get; set; }
        public double? price { get; set; }
    }
}
