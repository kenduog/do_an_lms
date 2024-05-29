using Entities;
using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;

namespace Request.RequestUpdate
{
    public class DiscountUpdate : DomainUpdate
    {
        public Guid? tuitionConfigId { get; set; }
        public string name { get; set; }
        /// <summary>
        /// 1 - Giảm tiền
        /// 2 - Giảm %
        /// </summary>
        public int? type { get; set; }
        [JsonIgnore]
        public string typeName
        {
            get
            {
                return tbl_Discount.GetTypeName(type ?? 0);
            }
        }
        /// <summary>
        /// Giá trị
        /// </summary>
        public double? value { get; set; }
        /// <summary>
        /// Giá trị tối đa => áp dụng cho loại %
        /// </summary>
        public double? maximumValue { get; set; }
        /// <summary>
        /// Mô tả
        /// </summary>
        public string description { get; set; }
    }
}
