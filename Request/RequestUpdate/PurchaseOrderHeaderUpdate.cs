using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;

namespace Request.RequestUpdate
{
    public class PurchaseOrderHeaderUpdate : DomainUpdate
    {
        public string code { get; set; }
        public double? date { get; set; }
        public string statusCode { get; set; }
        public string description { get; set; }
        public List<PurchaseOrderLineUpdate> details { get; set; }
    }
    public class ChangeStatusPurchase : DomainUpdate
    {
        public string statusCode { get; set; }
    }
}
