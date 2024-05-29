using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities
{
    public class tbl_FoodItem : DomainEntities.DomainEntities
    {
        public Guid? foodId { get; set; }
        public Guid? itemId { get; set; }
        public double? qty { get; set; }
        public double? essenceRate { get; set; }
        public double? actualQty { get; set; }
        public double? calo { get; set; }
        public double? protein { get; set; }
        public double? lipit { get; set; }
        public double? gluxit { get; set; }

        [NotMapped]
        public string itemName { get; set; }
        [NotMapped]
        public string unitOfMeasureName { get; set; }
    }
}
