using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestUpdate
{
    public class CommentInNewsUpdate : DomainUpdate
    {
        /// <summary>
        /// Nội dung
        /// </summary>
        public string content { get; set; }
    }
}
