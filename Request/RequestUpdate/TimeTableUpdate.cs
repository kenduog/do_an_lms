using Request.DomainRequests;
using Request.RequestCreate;
using System;
using System.Collections.Generic;

namespace Request.RequestUpdate
{
    public class TimeTableUpdate : DomainUpdate
    {
        public string name { get; set; }
        public bool? active { get; set; }
    }
}
