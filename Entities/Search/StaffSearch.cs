using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Entities.Search
{
    public class StaffSearch : BaseSearch
    {
        /// <summary>
        /// 1 - Màn hình giáo viên
        /// 2 - Màn hình nhân viên
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// Chinh nhánh
        /// </summary>
        public string branchId { get; set; }
        /// <summary>
        /// Nhóm quyền
        /// </summary>
        public string groupIds { get; set; }
        /// <summary>
        /// 1 - Thời gian giảm dần 
        /// 2 - Thời gian tăng dần
        /// 3 - Tên giảm dần 
        /// 4 - Tên tăng dần
        /// </summary>
    }

    
}
