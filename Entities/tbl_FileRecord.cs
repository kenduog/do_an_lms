using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_FileRecord : DomainEntities.DomainEntities
    {
        /// <summary>
        /// Năm học bắt đầu
        /// </summary>
        public Guid? schoolYearId { get; set; }
        public string code { get; set; }
        public string name { get; set; }

        /// <summary>
        /// từ viết tắt cho tên lớp
        /// </summary>
        public string acronym { get; set; }
    }
}
