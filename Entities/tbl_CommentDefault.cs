using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_CommentDefault : DomainEntities.DomainEntities
    {
        /// <summary>
        /// Tiêu đề mẫu nhận xét
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? branchId { get; set; }
        /// <summary>
        /// Buổi ăn trưa
        /// </summary>
        public string lunch { get; set; }
        /// <summary>
        /// Buổi ăn chiều nhẹ
        /// </summary>
        public string afternoonSnack { get; set; }
        /// <summary>
        /// Buổi ăn chiều 
        /// </summary>
        public string afternoon { get; set; }
        /// <summary>
        /// Ngủ 
        /// </summary>
        public string sleep { get; set; }
        /// <summary>
        /// Vệ sinh 
        /// </summary>
        public string hygiene { get; set; }
        [NotMapped]
        public string branchName { get; set; }
        [NotMapped]
        public string userCreateName { get; set; }
        [NotMapped]
        public string userUpdateName { get; set; }
    }
}
