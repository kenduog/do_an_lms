
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class tbl_DeliveryOrderHeader : DomainEntities.DomainEntities
    {
        public string code { get; set; }
        public double? date { get; set; }
        public string typeCode { get; set; }
        public string statusCode { get; set; }
        public Guid? branchId { get; set; }
        //public Guid? vendorId { get; set; }
        [NotMapped]
        public string vendorName { get; set; }
        public string description { get; set; }
        public double? totalProduct { get; set; }
        //public double? totalItem { get; set; }
        public double? amt { get; set; }
        public bool? isApproved { get; set; }
        public Guid? approvedBy { get; set; }
        public double? approvedDate { get; set; }

        public virtual ICollection<tbl_DeliveryOrderLine> details { get; set; }
    }
}
