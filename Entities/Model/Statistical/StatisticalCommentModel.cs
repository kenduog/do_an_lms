using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Model.Statistical
{
    // bổ sung thêm nhận xét tăng hay giảm so với năm trước
    public class StatisticalCommentModel : StatisticalModel
    {
        /// <summary>
        /// số lượng chênh lệch
        /// </summary>
        public double? DifferenceQuantity { get; set; }
        /// <summary>
        /// tỷ lệ chênh lệch
        /// </summary>
        public double? DifferenceValue { get; set; }
        /// <summary>
        /// 1 - tăng
        /// 2 - giảm
        /// 3 - không đổi
        /// </summary>
        public int? Status { get; set; }
        public string StatusName
        {
            get
            {
                return Status == 1 ? "tăng"
                    : Status == 2 ? "giảm"
                    : Status == 3 ? "không đổi" : null;
            }
        }
    }
}
