using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities.Search
{
    public class ContentTypeSearch : BaseSearch
    {
        public Guid? parentId { get; set; }
        public bool isRoot
        {
            get
            {
                return parentId.HasValue ? false : true;
            }
        }
    }
}
