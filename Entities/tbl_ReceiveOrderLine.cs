
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class tbl_ReceiveOrderLine : DomainEntities.DomainEntities
    {
        public Guid? receiveOrderId { get; set; }
        [ForeignKey(nameof(receiveOrderId))]
        public virtual tbl_ReceiveOrderHeader receiveOrderHeader { get; set; }
        public Guid? itemId { get; set; }
        public Guid? itemSkuId { get; set; }
        public Guid? unitOfMeasureId { get; set; }
        public double? unitPrice { get; set; }
        public double? unitPriceInventory { get; set; }
        public double? qty { get; set; }
        public double? amt { get; set; }
        public double? convertQty { get; set; }

        [NotMapped]
        public string name { get; set; }
        [NotMapped]
        public string unitOfMeasureName { get; set; }
    }
}
