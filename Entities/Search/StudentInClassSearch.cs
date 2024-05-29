using Entities.DomainEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities.Search
{
    public class StudentInClassSearch : BaseSearch
    {
        /// <summary>
        /// Trạng thái học sinh trong lớp
        /// 1 - Đang học
        /// 2 - Đã tốt nghiệp
        /// 3 - Thôi học
        /// 4 - Bảo lưu
        /// 5 - Học lại
        /// </summary>
        public int status { get; set; }
        public Guid? classId { get; set; }
        public Guid? studentId { get; set; }
    }
    public class StudentAvailableSearch
    {
        [Required(ErrorMessage = MessageContants.req_classId)] 
        public Guid? classId { get; set; }
        [JsonIgnore]
        public Guid? schoolYearId { get; set; }
        

    }
}
