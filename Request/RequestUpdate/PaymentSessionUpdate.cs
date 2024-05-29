using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestUpdate
{
    public class PaymentSessionUpdate : DomainUpdate
    {
        /// <summary>
        /// Lý do 
        /// </summary>
        public string reason { get; set; }
        public string note { get; set; }
        /// <summary>
        /// 1 - Chờ duyệt 
        /// 2 - Đã duyệt
        /// 3 - Không duyệt
        /// </summary>
        public int? status { get; set; }
        public string statusName { get { return status == 1 ? "Chờ duyệt" : status == 2 ? "Đã duyệt" : status == 3 ? "Không duyệt" : null; } }
    }
}
