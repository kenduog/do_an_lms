using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.Search
{
    public class FeedbackPermissionSearch : BaseSearch
    {
        public Guid? branchId { get; set; }
    }
}
