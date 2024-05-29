using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.DomainRequests;
using Request.RequestUpdate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface IGoodBehaviorCertificateService : IDomainService<tbl_GoodBehaviorCertificate, GoodBehaviorCertificateSearch>
    {
        Task<PagedList<tbl_GoodBehaviorCertificate>> GetOrGenerateGoodBehaviorCertificate(GoodBehaviorCertificateSearch request);
        Task<LearningResultByWeek> GetByWeek(WeekReportRequest request);

        Task<IList<tbl_GoodBehaviorCertificate>> GetForNoti(GoodBehaviorCertificateSearch request);
        Task SendNotification(GoodBehaviorNotificationRequest request);
    }
}
