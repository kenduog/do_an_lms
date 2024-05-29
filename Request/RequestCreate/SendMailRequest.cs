using Entities;
using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class SendMailRequest
    {
        public string from { get; set; }
        public string to { get; set; }
        public string subject { get; set; }
        public IList<string> ccs { get; set; }
        public IList<string> bccs { get; set; }
        public EmailContent content { get; set; }
    }
}
