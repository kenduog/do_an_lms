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
    public class PermissionCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_contentType)]
        public Guid? contentTypeId { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        /// <summary>
        /// API to call
        /// </summary>
        public string actions { get; set; }
        public string entity { get; set; }
    }
}
