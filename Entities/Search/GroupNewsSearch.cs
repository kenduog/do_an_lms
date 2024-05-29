using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Entities.Search
{
    public class GroupNewsSearch : BaseSearch
    {
        public Guid? userId { get; set; }
    }
}
