using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Entities
{
    public class tbl_Feedback : DomainEntities.DomainEntities
    {
        /// <summary>
        /// Id nhóm phản hồi
        /// </summary>
        public Guid? feedbackGroupId { get; set; }
        /// <summary>
        /// Tiêu đề phản hồi
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Nội dung phản hồi
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// Trạng thái phản hồi
        /// 1 Mới gửi
        /// 2 Đang xử lý
        /// 3 Đã xong
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// Cờ độ ưu tiên
        /// </summary>
        public bool? priority { get; set; }
        /// <summary>
        /// Số sao
        /// </summary>
        public int? numberOfStars { get; set; }
        /// <summary>
        /// Cờ ẩn danh
        /// </summary>
        public bool? incognito { get; set; }
        [NotMapped]
        public string feedbackGroupName_vi { get; set; }
        [NotMapped]
        public string createdByName { get; set; }
        [NotMapped]
        public string thumbnail { get; set; }
        [NotMapped]
        public string thumbnailResize { get; set; }
        [NotMapped]
        public string email { get; set; }
        [NotMapped]
        public string phone { get; set; }
        [NotMapped]
        public string address { get; set; }
        [NotMapped]
        public List<tbl_FeedbackReply> replies { get; set; }

        [NotMapped]
        [JsonIgnore]
        public int allState { get; set; }
        [NotMapped]
        [JsonIgnore]
        public int processing { get; set; }
        [NotMapped]
        [JsonIgnore]
        public int done { get; set; }
        [NotMapped]
        [JsonIgnore]
        public int sent { get; set; }
    }

    //public class FeedbackOverview
    //{
    //    public List<ChartData> groupChart { get; set; }
    //    public List<ChartData> roleChart { get; set; }
    //    public List<ChartData> starChart { get; set; }
    //}

    public class FeedbackPagedList : PagedList<tbl_Feedback>
    {
        public int allState { get; set; }
        public int processing { get; set; }
        public int done { get; set; }
        public int sent { get; set; }
    }
}
