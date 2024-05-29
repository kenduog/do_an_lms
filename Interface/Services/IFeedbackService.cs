using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface IFeedbackService : IDomainService<tbl_Feedback, FeedbackSearch>
    {
        Task<FeedbackPagedList> GetPagedStatus(FeedbackSearch baseSearch);
        Task<List<tbl_Feedback>> GetFeedbackReport();
        Task Done(Guid id);
    }
}
