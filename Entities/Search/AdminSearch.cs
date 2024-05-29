using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class AdminSearch : BaseSearch
    {
        /// <summary>
        /// 1 - Thời gian giảm dần 
        /// 2 - Thời gian tăng dần
        /// 3 - Tên giảm dần 
        /// 4 - Tên tăng dần
        /// </summary>
        [DefaultValue(0)]
        public override int orderBy { set; get; }
    }
}
