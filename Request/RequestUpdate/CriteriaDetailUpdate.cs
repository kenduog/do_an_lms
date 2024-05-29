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
    public class CriteriaDetailUpdate : DomainUpdate
    {
        public string code { get; set; }
        public string name { get; set; }
    }
}
