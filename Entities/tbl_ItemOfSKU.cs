
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class tbl_ItemOfSKU : DomainEntities.DomainEntities
    {

        public Guid? itemId { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string nameShort { get; set; }
        public Guid? unitOfMearsureId { get; set; }
        public bool? isMain { get; set; }
        /// <summary>
        /// So luong quy doi
        /// </summary>
        public double? convertQty { get; set; }
        public double? limitInventory { get; set; }
        public string description { get; set; }
        public string thumbnail { get; set; }
        public double? unitPrice { get; set; }

        [NotMapped]
        public string unitOfMeasureName { get; set; }

    }
}
