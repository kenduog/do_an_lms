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
    public class tbl_MenuItem : DomainEntities.DomainEntities
    {
        public Guid? menuId { get; set; }
        public Guid? itemId { get; set; }
        public double? qty { get; set; }
        public double? price { get; set; }
        public double? amt { get; set; }
        public Guid? unitOfMeasureId { get; set; }

        [NotMapped]
        public string itemName { get; set; }
        [NotMapped]
        public string unitOfMeasureName { get; set; }
        //not mapped any field to show calo, lipit, gluxit, protein
        [NotMapped]
        public double? calo { get; set; }
        [NotMapped]
        public double? gluxit { get; set; }
        [NotMapped]
        public double? lipit { get; set; }
        [NotMapped]
        public double? protein { get; set; }
        [NotMapped]
        public double? unitPrice { get; set; }
    }
}
