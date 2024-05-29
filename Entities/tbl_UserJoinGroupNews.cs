using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_UserJoinGroupNews : DomainEntities.DomainEntities
    {
        /// <summary>
        /// Tên nhóm
        /// </summary>
        public Guid groupNewsId { get; set; }
        /// <summary>
        /// Thành viên
        /// </summary>
        public Guid userId { get; set; }
        /// <summary>
        /// 1 - owner
        /// 2 - member
        /// 3 - admin group
        /// </summary>
        public int? userType { get; set; }
        /// <summary>
        /// 0 đã tham gia nhóm
        /// 1 đã rời nhóm
        /// </summary>
        public bool isHide { get; set; }
    }
}
