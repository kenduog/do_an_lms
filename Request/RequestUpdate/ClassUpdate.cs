using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestUpdate
{
    public class ClassUpdate : DomainUpdate
    {
        public string name { get; set; }
        public int? size { get; set; }
        public Guid? teacherId { get; set; }
        public Guid? schoolYearId { get; set; }
        public List<ClassShiftUpdate> shifts { get; set; }
    }
    public class ClassShiftUpdate
    {
        /// <summary>
        /// Tạo mới để null
        /// Chỉnh sửa và xóa truyền Id cũ
        /// </summary>
        public Guid? id { get; set; }
        [Required(ErrorMessage = MessageContants.req_day)]
        public int? day { get; set; }
        /// <summary>
        /// tiết
        /// </summary>
        [Required(ErrorMessage = MessageContants.req_period)]
        public int? period { get; set; }
        /// <summary>
        /// Giờ bắt đầu, tính tại 01/01/1970 GMT
        /// </summary>
        public double? sTime { get; set; }
        /// <summary>
        /// Giờ kết thúc, tính tại 01/01/1970 GMT
        /// </summary>
        [LargerStartTime]
        public double? eTime { get; set; }
    }
}
