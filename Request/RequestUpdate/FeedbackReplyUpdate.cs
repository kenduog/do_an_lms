using Request.DomainRequests;

namespace Request.RequestCreate
{
    public class FeedbackReplyUpdate : DomainUpdate
    {
        public string content { get; set; }
    }
}
