using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities
{
    public class tbl_CollectionSessionLine : DomainEntities.DomainEntities
    {
        public Guid? collectionSessionHeaderId { get; set; }
        public Guid? collectionPlanId { get; set; }
        public Guid? collectionSessionId { get; set; }
        public Guid? feeId { get; set; }
        [NotMapped]
        public string feeName { get; set; }
        public int? collectionType { get; set; }
        public double? price { get; set; }
        public string description { get; set; }
    }
}
