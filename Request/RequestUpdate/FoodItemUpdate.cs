using Request.DomainRequests;
using System;

namespace Request.RequestUpdate
{
    public class FoodItemUpdate : DomainUpdate
    {
        public Guid itemId { get; set; }
        public double? qty { get; set; }
        public double? essenceRate { get; set; }
        public double? weightPerUnit { get; set; }
        public double? actualQty { get; set; }
        public double? calo { get; set; }
        public double? lipit { get; set; }
        public double? protein { get; set; }
        public double? gluxit { get; set; }
    }
}
