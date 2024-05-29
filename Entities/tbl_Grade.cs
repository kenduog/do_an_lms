using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities
{
    public class tbl_Grade: DomainEntities.DomainEntities
    {
        public Guid? schoolYearId { get; set; }
        public Guid? branchId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        /// <summary>
        /// Từ viết tắt cho tên lớp
        /// </summary>
        public string acronym { get; set; }
        /// <summary>
        /// Tuổi tối thiểu của học sinh
        /// </summary>
        public int? studentYearOld { get; set; }
    }
}
