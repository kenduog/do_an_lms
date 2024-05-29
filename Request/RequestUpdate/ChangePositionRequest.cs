using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestUpdate
{
    public class ChangePositionRequest
    {
        public List<ChangePositionItem> items { get; set; }
    }
    public class ChangePositionItem
    {
        [Required(ErrorMessage = "Không tìm thấy dữ liệu")]
        public Guid? id { get; set; }  
        public int position { get; set; }
    }
}
