using Entities;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class TuitionConfigCreate : DomainCreate
    {
        [Required(ErrorMessage = "Vui lòng chọn loại khoản thu")]
        public Guid? tuitionConfigCategoryId { get; set; }
        public string name { get; set; }
        public string thumbnail { get; set; }
        public double? price { get; set; }
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// mô tả
        /// </summary>
        public string description { get; set; }
    }
}
