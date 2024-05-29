
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class tbl_ItemInventory : DomainEntities.DomainEntities
    {
        public Guid? itemId { get; set; }
        public Guid? branchId { get; set; }
        public double? qty { get; set; }
        public double? unitPrice { get; set; }

        [NotMapped]
        public string itemCode { get; set; }
        [NotMapped]
        public string itemName { get; set; }
        [NotMapped]
        public string itemGroupName { get; set; }
        [NotMapped]
        public string unitOfMeasureName { get; set; }
    }

    public class ItemInventoryExport
    {
        [NotMapped]
        public string itemCode { get; set; }
        [NotMapped]
        public string itemName { get; set; }
        [NotMapped]
        public string itemGroupName { get; set; }
        [NotMapped]
        public string unitOfMeasureName { get; set; }
        public double? qty { get; set; }
    }

    public class InventoryDetailBySKU
    {
        public Guid id { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string unitOfMeasureName { get; set; }
        public double? convertQty { get; set; }
        public double? qty { get; set; }
    }
}
