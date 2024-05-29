using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class tbl_StudySession : DomainEntities.DomainEntities
    {
        public Guid? branchId { get; set; }
        /// <summary>
        /// Tiết học (từ 1 tới 12)
        /// </summary>
        [Range(1,12)]
        public int? index { get; set; }
        public double? sTime { get; set; }
        public double? eTime { get; set; }
    }
}
