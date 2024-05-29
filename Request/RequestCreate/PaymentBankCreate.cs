using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class PaymentBankCreate : DomainCreate
    {
        [Required (ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
        public string icon { get; set; }
        public string thumbnail { get; set; }
        public string description { get; set; }
    }
}
