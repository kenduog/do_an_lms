using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class TuitionConfigSearch : BaseSearch
    {
        public Guid? tuitionConfigCategoryId { get; set; }
    }
}
