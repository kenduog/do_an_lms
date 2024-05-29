using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_ClassShift : DomainEntities.DomainEntities
    {
        public Guid? classId { get; set; }
        /// <summary>
        /// Thứ
        /// </summary>
        public int? day { get; set; }
        /// <summary>
        /// tiết
        /// </summary>
        public int? period { get; set; }
        /// <summary>
        /// Giờ bắt đầu, tính tại 01/01/1970 GMT
        /// </summary>
        public double? sTime { get; set; }
        /// <summary>
        /// Giờ kết thúc, tính tại 01/01/1970 GMT
        /// </summary>
        public double? eTime { get; set; }
    }
}
