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
    public class AvailableStudentRequest : StudentSearch
    {
        /// <summary>
        /// Năm học
        /// </summary>
        [Required(ErrorMessage = MessageContants.req_gradeId)]
        public Guid? schoolYearId { get; set; }
      
        [Required(ErrorMessage = MessageContants.req_gradeId)]
        public Guid? gradeId { get; set; }

        /// <summary>
        /// Lọc ra những học viên đủ điều kiện xếp lớp
        /// </summary>
        [Required(ErrorMessage = MessageContants.req_isNewStudent)]
        public bool? isNewStudent { get; set; }
        public bool? isAvailable { get; set; }
        [JsonIgnore]
        public int? minYearOld { get; set; }
    }
}
