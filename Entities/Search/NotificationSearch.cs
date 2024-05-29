using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Entities.Search
{
    public class NotificationSearch : BaseSearch
    {
        public Guid? userId { get; set; }
        public bool? isSeen { get; set; }
    }
}
