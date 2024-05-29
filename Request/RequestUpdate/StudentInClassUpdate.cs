using Entities;
using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;


namespace Request.RequestUpdate
{
    public class StudentInClassUpdate : DomainUpdate
    {

        /// <summary>
        /// Trạng thái học sinh trong lớp
        /// 1 - Đang học
        /// 2 - Đã tốt nghiệp
        /// 3 - Thôi học
        /// 4 - Bảo lưu
        /// 5 - Học lại
        /// </summary>
        public int? status { get; set; }
        [JsonIgnore]
        public string statusName { get { return tbl_StudentInClass.GetStatusName(status ?? 0); } }
        public string note { get; set; }
    }
}
