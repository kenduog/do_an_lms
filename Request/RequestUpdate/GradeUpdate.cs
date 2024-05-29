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
    public class GradeUpdate : DomainUpdate
    {
        public string code { get; set; }
        public string name { get; set; }

        /// <summary>
        /// từ viết tắt cho tên lớp
        /// </summary>
        public string acronym { get; set; }
        public int? level { get; set; }
        public int? studentYearOld { get; set; }
    }
}
