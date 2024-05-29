using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_DocumentNews : DomainEntities.DomainEntities
    {
        public Guid? newsId { get; set; }
        public Guid? commentId { get; set; }
        /// <summary>
        /// Link hình ảnh, video, audio, tài liệu,..
        /// </summary>
        public string link { get; set; }
        /// <summary>
        /// Loại đuôi file
        /// </summary>
        public string typeCode { get; set; }
    }
}
