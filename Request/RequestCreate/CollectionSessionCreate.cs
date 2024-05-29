using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class CollectionSessionCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_collectionPlan)]
        public Guid? collectionPlanId { get; set; }
        [Required(ErrorMessage = MessageContants.req_name)]
        public string name { get; set; }
        [Required(ErrorMessage = MessageContants.req_month)]
        public int? month { get; set; }
        [Required(ErrorMessage = MessageContants.req_year)]
        public int? year { get; set; }
        public string description { get; set; }

        public List<Guid?> studentIds { get; set; }
        public List<FeeValuePair> fees { get; set; }
    }

    public class FeeValuePair
    {
        public Guid? feeId { get; set; }
        public int? collectionType { get; set; }
        public double? price { get; set; }
        public string description{ get; set; }
    }

    public class CollectionSessionNotificationRequest
    {
        [Required(ErrorMessage = MessageContants.req_collectionSessionId)]
        public Guid? collectionSessionId { get; set; }
        [Required(ErrorMessage = MessageContants.req_notificationTitle)]
        public string title { get; set; }
        [Required(ErrorMessage = MessageContants.req_notificationContent)]
        public string content { get; set; }
    }
}
