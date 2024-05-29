using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestUpdate
{
    public class ItemOfSKUUpdate : DomainUpdate
    {
        public string code { get; set; }
        public string name { get; set; }
        public string nameShort { get; set; }
        public Guid? unitOfMearsureId { get; set; }
        public bool? isMain { get; set; }
        /// <summary>
        /// So luong quy doi
        /// </summary>
        public double? convertQty { get; set; }
        public double? limitInventory { get; set; }
        public string description { get; set; }
        public string thumbnail { get; set; }
    }
}
