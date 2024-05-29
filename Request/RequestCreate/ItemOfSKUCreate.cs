using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utilities;

namespace Request.RequestCreate
{
    public class ItemOfSKUCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_itemId)]
        public Guid? itemId { get; set; }
        public string code { get; set; }
        [Required(ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
        public string nameShort { get; set; }
        [Required(ErrorMessage = MessageContants.req_unitOfMeasureId)]
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
