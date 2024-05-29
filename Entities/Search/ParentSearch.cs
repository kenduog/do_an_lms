using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class ParentSearch : BaseSearch
    {
        public Guid? studentId { get; set; }
        /// <summary>
        /// 1 - Thời gian giảm dần 
        /// 2 - Thời gian tăng dần
        /// 3 - Tên giảm dần 
        /// 4 - Tên tăng dần
        /// </summary>
        [DefaultValue(0)]
        public override int orderBy { set; get; }
        /// <summary>
        /// 1 - Cha 
        /// 2 - Mẹ 
        /// 3 - Người giám hộ
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// Chinh nhánh
        /// </summary>
        public string branchId { get; set; }
        /// <summary>
        /// Tự lọc theo chi nhánh của người dùng
        /// </summary>
        public string myBranchIds { get; set; }
    }
}
