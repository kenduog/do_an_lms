
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Utilities;

namespace Entities
{
    public class tbl_ValueEntry : DomainEntities.DomainEntities
    {
        public double? date { get; set; }
        public Guid? branchId { get; set; }
        /// <summary>
        /// 1 - Nhap kho
        /// 2 - Xuat kho
        /// </summary>
        public int? type{ get; set; }
        public Guid? itemId { get; set; }
        public Guid? itemSkuId { get; set; }
        public Guid? billId{ get; set; }
        public Guid? vendorId { get; set; }
        public double? price { get; set; }
        public double? qty { get; set; }
        public double? convertQty { get; set; }

        [NotMapped]
        public double? importQty{ get; set; }
        [NotMapped]
        public double? exportQty{ get; set; }
        [NotMapped]
        public string vendorName { get; set; }
        [NotMapped]
        public string itemName { get; set; }
        [NotMapped]
        public string billCode { get; set; }
        [NotMapped]
        public string itemCode { get; set; }
        [NotMapped]
        public string typeName { get; set; }
    }

    public class ValueEntryExport
    {
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string date { get; set; }
        public string typeName { get; set; }
        public string billCode { get; set; }
        public string vendorName { get; set; }
        public double? importQty { get; set; }
        public double? exportQty { get; set; }
    }

    public class ValueEntryReport
    {
        public double? beginning { get; set; }
        public double? duringExport { get; set; }
        public double? duringImport { get; set; }
        public double? ending { get; set; }
        public PagedList<tbl_ValueEntry> data { get; set; }
    }
}
