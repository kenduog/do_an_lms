using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestUpdate
{
    public class TypeDocumentNewsUpdate : DomainUpdate
    {
        /// <summary>
        /// Tên loại file
        /// </summary>
        public string typeName { get; set; }
    }
}
