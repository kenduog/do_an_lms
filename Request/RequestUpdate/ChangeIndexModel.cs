using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestUpdate
{
    public class ChangeIndexModel
    {
        [Required(ErrorMessage = "Không tìm thấy dữ liệu")]
        public List<ChangeIndexItem> items { get; set; }
    }
    public class ChangeIndexItem
    { 
        public Guid id { get; set; }
        public int index { get; set; }
    }
}
