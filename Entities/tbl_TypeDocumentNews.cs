using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_TypeDocumentNews : DomainEntities.DomainEntities
    {
        /// <summary>
        /// Loại file: 
        /// 0 - Hình ảnh
        /// 1 - Video
        /// 2 - Audio
        /// 3 - Tài liệu (document)
        /// </summary>
        public int? typeCode { get; set; }
        public string typeName { get; set; }
    }
}
