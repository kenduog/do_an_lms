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
    public class ItemOfSKUSearch : BaseSearch
    {
        [Required(ErrorMessage = MessageContants.req_itemId)]
        public Guid? itemId { get; set; }
        public Guid? unitOfMeasureId { get; set; }
    }
}
