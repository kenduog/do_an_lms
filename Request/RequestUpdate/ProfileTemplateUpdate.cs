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
    public class ProfileTemplateUpdate : DomainUpdate
    {
        public string name { get; set; }
        public int? type { get; set; }
    }
    public class ProfileTemplatePositionUpdate : DomainUpdate
    {
        public bool isUp { get; set; } = true;
    }

}
