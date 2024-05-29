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
    public class CommentSearch : BaseSearch
    {
        public Guid? branchId { get; set; }
        public Guid? classId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày nhận xét")]
        public double? date { get; set; }
        /// <summary>
        /// Mã học sinh - Dùng cho mobile app
        /// </summary>
        public Guid? studentId { get; set; }
    }
}
