using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.AuthEntities
{
    public class tbl_ContentType : DomainEntities.DomainEntities
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
        public bool? isRoot { get; set; } 

        /// <summary>
        /// Id menu cha
        /// </summary>
        public Guid? parentId { get; set; }

        [NotMapped]
        public IList<tbl_ContentType> childs { get; set; }
        [NotMapped]
        public bool hasChild { get; set; }
        [NotMapped]
        public bool allowed { get; set; }
        public string route { get; set; }
    }
}
