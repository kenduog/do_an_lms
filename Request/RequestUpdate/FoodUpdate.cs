using Request.DomainRequests;
using System;
using System.Collections.Generic;

namespace Request.RequestUpdate
{
    public class FoodUpdate : DomainUpdate
    {
        public Guid? branchId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<FoodItemUpdate> items { get; set; }
    }
}
