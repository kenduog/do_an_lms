using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class TypeDocumentNewsCreate : DomainCreate
    {
        /// <summary>
        /// Tên loại file
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nội dung")]
        public string typeName { get; set; }
    }
}
