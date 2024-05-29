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
    public class MenuFoodCreate : DomainCreate
    {
        public Guid? foodId { get; set; }
        /// <summary>
        /// Buổi trong ngày
        /// 1 - Trưa
        /// 2 - Chiều
        /// 3 - Phụ chiều
        /// </summary>
        public int? type { get; set; }
    }
}
