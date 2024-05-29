using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Search
{
    public class WardsSearch : BaseSearch
    {
        public Guid? districtId { get; set; }
    }
}
