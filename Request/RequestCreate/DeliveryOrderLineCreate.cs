using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class DeliveryOrderLineCreate : DomainCreate
    {
        public Guid? itemId { get; set; }
        public Guid? itemSkuId { get; set; }
        public Guid? unitOfMeasureId { get; set; }
        public double? unitPrice { get; set; }
        public double? qty { get; set; }
        public double? amt { get; set; }
        public double? convertQty { get; set; }
    }
}
