using System;

namespace API.Model
{
    public class BillDetailDTO
    {
        public Guid id { get; set; }
        public Guid? billId { get; set; }
        /// <summary>
        /// Khoản thu
        /// </summary>
        public Guid? tuitionConfigDetailId { get; set; }
        public double? price { get; set; }
        public string note { get; set; }
        public string name { get; set; }
    }
}
