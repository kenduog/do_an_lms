using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestUpdate
{
    public class PaymentBankUpdate : DomainUpdate
    {
        public string name { get; set; }
        public string icon { get; set; }
        public string thumbnail { get; set; }
        public string description { get; set; }
    }
}
