using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Search
{
    public class FeedbackSearch : BaseSearch
    {
        public Guid? branchId { get; set; }
        /// <summary>
        /// Lọc theo trạng thái
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// Lọc theo người tạo
        /// </summary>
        public Guid? createdBy { get; set; }
        public string roleCode { get; set; }
        public Guid? groupId { get; set; }
        public double? sDate { get; set; }
        public double? eDate { get; set; }
    }
    public class FeedbackReportRequest
    {
        public double sDate { get; set; }   
        public double eDate { get; set; }
        public Guid? feedbackGroupId{ get; set; }
    }
}
