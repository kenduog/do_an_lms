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
    public class ContentTypeUpdate : DomainUpdate
    {
        /// <summary>
        /// Tên màn hình
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// controller
        /// </summary>
        public string code { get; set; }
        public int order { get; set; }

        /// <summary>
        /// Menu gốc
        /// </summary>
        public bool isRoot { get; set; } = false;

        /// <summary>
        /// Id menu cha
        /// </summary>
        public Guid? parentId { get; set; }
        public string route { get; set; }
    }
}
