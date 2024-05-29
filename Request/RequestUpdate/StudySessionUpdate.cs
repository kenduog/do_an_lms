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
    public class StudySessionUpdate : DomainUpdate
    {
        public double? sTime { get; set; }
        public double? eTime { get; set; }
    }
}
