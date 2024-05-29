using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.DomainRequests
{
    public class SendMailModel
    {
        public string to { get; set; }
        public string title { get; set; }
        public string content { get; set; }
    }
}
