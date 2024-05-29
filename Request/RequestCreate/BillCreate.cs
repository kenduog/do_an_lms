using Newtonsoft.Json;
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
    public class BillCreate : DomainCreate
    {
        /// <summary>
        /// 1 - khối
        /// 2 - lớp
        /// 3 - học viên
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn phân loại")]
        public int? type { get; set; }
        /// <summary>
        /// danh sách id
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn đối tượng")]
        public List<Guid> itemIds { get; set; }
        /// <summary>
        /// danh sách khoản thu
        /// </summary>
        public List<BillDetailModel> billDetails { get; set; }
        /// <summary>
        /// Thời gian thu
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập thời gian thu")]
        public double? date { get; set; }
        /// <summary>
        /// ghi chú
        /// </summary>
        public string Note { get; set; }
    }

    public class BillDetailModel
    {
        /// <summary>
        /// khoản thu
        /// </summary>
        public Guid tuitionConfigId { get; set; }
        /// <summary>
        /// số lượng
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// giảm giá
        /// </summary>
        public Guid? discountId { get; set; }
    }
}
