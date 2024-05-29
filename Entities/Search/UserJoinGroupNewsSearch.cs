using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities.Search
{
    public class UserJoinGroupNewsSearch : BaseSearch
    {
        [Required(ErrorMessage = "Vui lòng chọn nhóm bảng tin!")]
        public Guid groupNewsId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn nhóm bảng tin!")]
        public int type { get; set; }
        public int admin { get; set; } = 0;

    }
}
