using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Request.RequestCreate
{
    public class RoomCreate : DomainCreate
    {
        public Guid? branchId { get; set; }
        public string name { get; set; }
    }
}
