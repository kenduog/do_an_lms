using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Model.Statistical
{
    /// <summary>
    /// dùng cho các biểu đồ show dữ liệu 12 tháng trong năm
    /// </summary>
    public class Statistical12MonthModel : StatisticalModel
    {
        public string Month { get; set; }
    }
}
