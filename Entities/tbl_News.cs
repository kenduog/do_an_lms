using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_News : DomainEntities.DomainEntities
    {
        public Guid? groupNewsId { get; set; }
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>
        public string content { get; set; }

        public bool? pinned { get; set; }
        public int? pinnedPosition { get; set; }
        /// <summary>
        /// Chi nhánh
        /// </summary>
        public string branchIds { get; set; }
        public Guid? userId { get; set; }
        [NotMapped]
        public string groupName { get; set; }
        [NotMapped]
        public string fullName{ get; set; }
        [NotMapped]
        public string userThumbnail { get; set; }
        [NotMapped]
        public int? countLike { get; set; }
        [NotMapped]
        public int? countComment { get; set; }
    }
}
