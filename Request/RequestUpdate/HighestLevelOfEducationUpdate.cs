using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class HighestLevelOfEducationUpdate : DomainUpdate
    {
        public string name { get; set; }
    }
}
