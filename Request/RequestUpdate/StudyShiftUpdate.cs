using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class StudyShiftUpdate : DomainUpdate
    {
        public double? sTime { get; set; }
        public double? eTime { get; set; }
    }
}
