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
    public class StudentLeaveRequestUpdate : DomainUpdate
    {
        public double? fromDate { get; set; }
        public double? toDate { get; set; }
        public string description { get; set; }
    }
}
