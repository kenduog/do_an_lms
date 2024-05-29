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
    public class PurchaseOrderHeaderCreate : DomainCreate
    {
        public string code { get; set; }
        public double? date { get; set; }
        public string statusCode { get; set; } = LookupConstant.TrangThai_DangMo;
        public Guid? branchId { get; set; }
        public string description { get; set; }
        public List<PurchaseOrderLineCreate> details { get; set; }
    }
}
