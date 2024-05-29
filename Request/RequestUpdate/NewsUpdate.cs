using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestUpdate
{
    public class NewsUpdate : DomainUpdate
    {
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>
        public string content { get; set; }
        public string branchIds { get; set; }
        public List<string> uploads { get; set; }

    }

    public class PinPositionUpdate 
    {
        public List<PinPistionItem> items {get;set;}
    }
    public class PinPistionItem
    {
        public Guid id {get;set;}
        public int pinnedPosition { get; set; }
    }
}
