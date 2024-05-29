using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestUpdate
{
    public class GroupFeedbackUpdate : DomainUpdate
    {
        /// <summary>
        /// Tên nhóm
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Mã
        /// </summary>
        public string code { get; set; }
    }
}
