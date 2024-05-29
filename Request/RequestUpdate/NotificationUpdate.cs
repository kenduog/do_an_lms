using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class NotificationUpdate : DomainUpdate
    {
        /// <summary>
        /// Loại cập nhật
        /// </summary>
        [Required]
        public int type { get; set; }
    }
}
