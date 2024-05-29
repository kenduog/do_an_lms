using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Notification : DomainEntities.DomainEntities
    {
        public string title { get; set; }
        public string content{ get; set; }
        [DefaultValue(false)]
        public bool isSeen{ get; set; }
        public Guid? userId{ get; set; }
        public string link { get; set; }
        public string deepLink { get; set; }
        public string screen { get; set; }
        public string param { get; set; }
        [NotMapped]
        public string fullName { get; set; }
        [NotMapped]
        public int totalItemIsNotSeen { get; set; }

    }
    public class NotificationResponse
    {
        public int total { get; set; }
        public List<tbl_Notification> notifications { get; set; }
    }
}
