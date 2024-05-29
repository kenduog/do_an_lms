using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_Food : DomainEntities.DomainEntities
    {
        public Guid? branchId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int? itemCount { get; set; }
        [NotMapped]
        public string unitOfMeasureName{ get; set; }
        [NotMapped]
        public double? qty { get; set; }
        [NotMapped]
        public double? calo { get; set; }
        [NotMapped]
        public double? protein { get; set; }
        [NotMapped]
        public double? lipit { get; set; }
        [NotMapped]
        public double? gluxit { get; set; }

        [NotMapped]
        public virtual ICollection<tbl_FoodItem> items { get; set; }
    }

    public class FoodItem
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public int? type { get; set; }
    }
}
