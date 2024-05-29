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
    public class CriteriaDetailSearch : BaseSearch
    {
        [Required(ErrorMessage = MessageContants.req_criteriaId)]
        public Guid criteriaId { get; set; }
    }
}
