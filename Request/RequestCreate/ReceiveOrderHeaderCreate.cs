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
    public class ReceiveOrderHeaderCreate : DomainCreate
    {
        public string code { get; set; }
        public double? date { get; set; }
        public string typeCode { get; set; }
        public string statusCode { get; set; } = LookupConstant.TrangThai_DangMo;
        public Guid? branchId { get; set; }
        public Guid? vendorId { get; set; }
        public string description { get; set; }
        public double? totalProduct { get; set; }
        public double? totalItem { get; set; }
        public double? amt { get; set; }
        public List<ReceiveOrderLineCreate> details { get; set; }
    }
}
