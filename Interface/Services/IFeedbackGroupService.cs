using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;

namespace Interface.Services
{
    public interface IFeedbackGroupService : DomainServices.IDomainService<tbl_FeedbackGroup, FeedbackGroupSearch>
    {
    }
}
