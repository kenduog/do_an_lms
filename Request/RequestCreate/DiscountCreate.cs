using Entities;
using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.ComponentModel.DataAnnotations;


namespace Request.RequestCreate
{
    public class DiscountCreate : DomainCreate
    {
        public Guid? tuitionConfigId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
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
        [Required(ErrorMessage = "Vui lòng nhập số tiền miễn giảm")]
        public double? value { get; set; }
        /// <summary>
        /// Giá trị tối đa => áp dụng cho loại %
        /// </summary>
        /// 
        private double? _maximumValue;
        public double? maximumValue
        {
            get 
            {
                return type == 1 ? value :
                       type == 2 ? _maximumValue : 0;
            }
            set
            {
                _maximumValue = value;
            }
        }
        /// <summary>
        /// Mô tả
        /// </summary>
        public string description { get; set; }
    }
}
