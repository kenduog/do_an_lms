using Entities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.RequestCreate;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface IFeedbackReplyService : IDomainService<tbl_FeedbackReply, FeedbackReplySearch>
    {
        Task Vote(FeedbackVote request);
    }
}
