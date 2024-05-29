using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.DomainRequests;
using Request.RequestCreate;
using Request.RequestUpdate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;


namespace Interface.Services
{
    public interface ICollectionSessionService : IDomainService<tbl_CollectionSession, CollectionSessionSearch>
    {
        Task CustomAddItem(CollectionSessionCreate model);
        Task UpdateFeeByStudent(CollectionSessionLineUpdate model);
        Task UpdateOtherFee(CollectionSessionHeaderUpdate model);
        Task<tbl_CollectionSession> CustomGetByIdAsync(CollectionSessionByIdSearch request);
        Task<PagedList<CollectionSessionHeaderMobile>> GetByParent(CollectionSessionByParentSearch request);
        Task<CollectionSessionHeaderMobileModel> GetCollectionHeaderByParent(Guid id);
        Task ConfirmPayment(ComfirmPayment model);
        Task SendNotification(CollectionSessionNotificationRequest request);
    }
}
